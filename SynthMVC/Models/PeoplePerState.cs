using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SynthMVC.DAL;
using SynthMVC.Models;

namespace SynthMVC.Models
{
    public class PeoplePerState
    {
        [Key]
        public States State { get; set; }

        public List<Person> PeopleInState { get; set; }

        public PeoplePerState()
        {

        }
    }
}