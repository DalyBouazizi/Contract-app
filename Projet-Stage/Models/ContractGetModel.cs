﻿namespace Projet_Stage.Models
{
    public class ContractGetModel
    {



        public int id { get; set; }
        public string Type { get; set; }
            public DateTime? Datedeb { get; set; }
            public DateTime? DateFin { get; set; }
        public decimal? Salaireb { get; set; }
        public decimal? Salairen { get; set; }
        public int EmployeeId { get; set; }
        
    


}
}
