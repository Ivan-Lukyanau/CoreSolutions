using System;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace CoreFireConsole.Models
{
    public class Dinosaur
    {
        private string name;
        private double height;

        public Dinosaur(string name, double height)
        {
            this.name = name;   
            this.height = height;
        }

        [JsonProperty("height")]
        public double Height { get => this.height; set => this.height = value; }
    }
}
