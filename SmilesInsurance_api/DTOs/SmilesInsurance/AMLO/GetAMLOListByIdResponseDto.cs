﻿using System.ComponentModel.DataAnnotations;
using System;

namespace SmilesInsurance_api.DTOs.SmilesInsurance.AMLO
{
    public class GetAMLOListByIdResponseDto
    {
        public Guid AMLOListId { get; set; }
        public Guid AMLOLetterId { get; set; }
        public string IdCardNo { get; set; }
        public string PassportNo { get; set; }
        public DateTime? BirthDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SearchText { get; set; }

        public string FirstName_eng { get; set; }

        public string LastName_eng { get; set; }
        public bool? IsBlacklist { get; set; }
        public string Remark { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}