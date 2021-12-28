using ApiEscola.Repository;
using ApiEscola.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiEscola
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

            //Este comando vai permitir, de forma global, que não fique num ciclo infinito ao serializar/deserializar um json
            services.AddMvc()
               .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
               .AddNewtonsoftJson(c => c.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddControllers();     
            
            services.AddTransient<CursoRepository>();
            services.AddTransient<ProfessorRepository>();
            services.AddTransient<MateriaRepository>();
            services.AddTransient<TurmaRepository>();
            services.AddTransient<AlunoRepository>();

            services.AddTransient<CursoService>();
            services.AddTransient<ProfessorService>();
            services.AddTransient<MateriaService>();
            services.AddTransient<TurmaService>();
            services.AddTransient<AlunoService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ExercicioApiEscola", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiEscola v1"));
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
}
