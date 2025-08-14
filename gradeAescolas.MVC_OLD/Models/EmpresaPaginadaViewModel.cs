﻿namespace gradeAescolas.MVC.Models;

public class EmpresaPaginadaViewModel
{
    public List<EmpresaViewModel> Empresas { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; } 
    public string? SearchTerm { get; set; }
}