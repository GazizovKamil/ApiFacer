using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFaiserScript.Classes
{
    public class Person
    {
        public string tag { get; set; }
        public int[] coord { get; set; }
        public double confidence { get; set; }
        public double awesomeness { get; set; }
        public double similarity { get; set; }
        public string sex { get; set; }
        public string emotion { get; set; }
        public int age { get; set; }
        public double valence { get; set; }
        public double arousal { get; set; }
        public double frontality { get; set; }
        public double visibility { get; set; }
    }

    public class Object
    {
        public int status { get; set; }
        public string name { get; set; }
        public List<Person> persons { get; set; }
    }

    public class Body
    {
        public List<Object> objects { get; set; }
    }

    public class Root
    {
        public int status { get; set; }
        public Body body { get; set; }
        public bool htmlencoded { get; set; }
        public long last_modified { get; set; }
    }
}
