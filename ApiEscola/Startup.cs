using ApiEscolaEfCore.Repository;
using ApiEscolaEfCore.Services;
using Dapper;
using Dominio;
using EfContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;

using System.Data;


namespace ApiEscolaEfCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("Conexao");

            //Este comando vai permitir, de forma global, que não fique num ciclo infinito ao serializar/deserializar um json
            services.AddMvc()
               .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
               .AddNewtonsoftJson(c => c.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddControllers();

            services.AddDbContext<EfContext.AppContext>(
                configuration =>
                {
                    configuration.UseOracle(connectionString,
                        opt =>
                        {
                            opt.MigrationsAssembly(typeof(EfContext.AppContext).Assembly.GetName().Name);
                        });
                });

            services.AddTransient<CursoRepository>();
            services.AddTransient<ProfessorRepository>();
            services.AddTransient<TurmaRepository>();
            services.AddTransient<AlunoRepository>();

            services.AddTransient<CursoService>();
            services.AddTransient<ProfessorService>();
            services.AddTransient<MateriaService>();
            services.AddTransient<TurmaService>();
            services.AddTransient<AlunoService>();

            services.AddTransient<ICursoRepository, DapperContext.Repository.CursoRepository>();
            services.AddTransient<IProfessorRepository, DapperContext.Repository.ProfessorRepository>();
            services.AddTransient<IMateriaRepository, DapperContext.Repository.MateriaRepository>();
            services.AddTransient<ITurmaRepository, DapperContext.Repository.TurmaRepository>();
            services.AddTransient<IAlunoRepository, DapperContext.Repository.AlunoRepository>();

            services.AddTransient<IMateriaRepositoryEfCore, EfContext.Repository.MateriaRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ExercicioApiEscolaEfCore", Version = "v1" });
            });

            //Os comandos abaixo sao para que o ef consiga manipular o guid, tranformando ele de guid para string e vice-versa. Se não,
            // da pt qunado a gente tentar fazer um select no banco e retorna um campo id (string) que na aplicação usamos ele como guid ai nessa conversao da pt.
            SqlMapper.AddTypeHandler(new MySqlGuidTypeHandler());
            SqlMapper.RemoveTypeMap(typeof(Guid));
            SqlMapper.RemoveTypeMap(typeof(Guid?));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiEscolaEfCore v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class MySqlGuidTypeHandler : SqlMapper.TypeHandler<Guid>
    {
        public override void SetValue(IDbDataParameter parameter, Guid guid)
        {
            parameter.Value = guid.ToString();
        }

        public override Guid Parse(object value)
        {
            return new Guid((string)value);
        }
    }
}
