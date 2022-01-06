using System;

namespace Dominio.Entities
{
    public class Base<TKey> 
    {
        public TKey Id { get; set; }
    }
}
