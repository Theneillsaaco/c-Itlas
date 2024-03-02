﻿using School.DAL.Context;
using School.DAL.Entities;
using School.DAL.Exceptions;
using School.DAL.Interfaces;
using School.DAL.Enums;



namespace School.DAL.Dao
{
    public class DaoDepartment : IDaoDepartment
    {
        private readonly SchoolContext context;
        public DaoDepartment (SchoolContext context) 
        {
            this.context = context;
        }
        public bool ExtistsDepartments(Func<Department, bool> filter)
        {
            return this.context.Departments.Any(filter);
        }

        public Department GetDepartment(int id)
        {
            return this.context.Departments.Find(id);
        }

        public List<Department> GetDepartments()
        {
            return this.context.Departments.ToList();
        }

        public List<Department> GetDepartments(Func<Department, bool> filter)
        {
            return this.context.Departments.Where(filter).ToList();
        }

        public void RemoveDepartment(Department department)
        {
            Department departmentToRemove = this.GetDepartment(department.DepartmentId);

            departmentToRemove.Deleted = department.Deleted;
            departmentToRemove.DeletedDate = department.DeletedDate;
            departmentToRemove.UserDeleted = department.UserDeleted;

            this.context.Departments.Update(departmentToRemove);

            this.context.SaveChanges();
        }

        public void SaveDerpartment(Department department)
        {
            string message = string.Empty;

            if (!IsDepartmentValid(department, ref message, Operations.Save))
                throw new DaoDepartmentException(message);

            this.context.Departments.Add(department);
            this.context.SaveChanges();
        }

        public void UpdateDepartment(Department department)
        {
            string message = string.Empty;

            if (!IsDepartmentValid(department, ref message, Operations.Update))
                throw new DaoDepartmentException(message);

            Department departmentToUpdate = this.GetDepartment(department.DepartmentId);

            departmentToUpdate.ModifyDate = department.ModifyDate;
            departmentToUpdate.Name = department.Name;
            department.StartDate = department.StartDate;
            departmentToUpdate.Budget = department.Budget;
            departmentToUpdate.Administrator = department.Administrator;
            departmentToUpdate.UserMod = department.UserMod;

            this.context.Departments.Add(departmentToUpdate);
            this.context.SaveChanges();
        }
        private bool IsDepartmentValid(Department department, ref string message, Operations operations)
        {
            bool result = false;

            if (string.IsNullOrEmpty(department.Name))
            {
                message = "El nombre del departamento es requerido";
                return true;
            }

            if (department.Name.Length > 50)
            {
                message = "El nombre es demaciado largo, el limite es 50 caracteres";
                return true;
            }

            if (department.Budget == 0)
            {
                message = "El presuesto no puede ser 0";
                return true;
            }
            if (this.ExtistsDepartments(cd => cd.Name == department.Name))
            {
                message = "El nombre ya existe";
                return true;
            }
             
            if (operations == Operations.Save)
            {
                if (this.ExtistsDepartments(cd => cd.Name == department.Name))
                {
                    message = "El departamento ya se encuentra registrado.";
                    return result;
                }
            }

            else
                result = true; 

            return result;
        }
    }
}
