﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Atelier_5.ViewModel
{
    public class FilteredListViewModel : INotifyPropertyChanged
    {
        private int _selectedFilter = 0;
        private readonly string[] _filters;
        private Model.EntrepriseEntities _context;

        public FilteredListViewModel(Model.EntrepriseEntities context)
        {
            _context = context;
            _filters = "Tout le staff,10$ et moins,Par date d'anniversaire,Par âge croissant,Commandes françaises,Prix moyen par catégorie".Split(',');

        }
        public IEnumerable<object> FilteredList
        {
            get
            {
                switch (this._selectedFilter)
                {
                    case 0:
                        return from employee in _context.Employees
                               select new
                               {
                                   Prénom = employee.First_Name,
                                   Nom = employee.Last_Name
                               };
                    case 1:
                        return from product in _context.Products
                               where product.Unit_Price < 10.0m
                               select new
                               {
                                   Produit = product.Product_Name,
                                   Prix = product.Unit_Price
                               };
                    case 2:
                        return from employee in _context.Employees
                               where employee.Birth_Date.HasValue == true
                               where employee.Birth_Date.Value.Month == 01
                               select new
                               {
                                   Prénom = employee.First_Name,
                                   Nom = employee.Last_Name,
                                   Date = employee.Birth_Date
                               };
                    case 3:
                        return from employee in _context.Employees
                               where employee.Birth_Date.HasValue == true
                               let date = employee.Birth_Date.Value
                               orderby date
                               //select employee;
                               select new
                               {
                                   Prénom = employee.First_Name,
                                   Nom = employee.Last_Name,
                                   Date = date

                               };
                    case 4:
                        return from customer in _context.Customers
                               join order in _context.Orders on customer.Customer_ID equals order.Customer_ID
                                   into orders
                               select new 
                               {
                                   customer.Contact_Name,
                                   OrderCount = orders.Count() 
                               };
                    case 5:

                        
                       return from produit in _context.Products
                                    group produit by produit.Category_ID into grp
                                    select new
                                    {
                                        Category = grp.Key,
                                        AveragePrice = grp.Average(produit => produit.Unit_Price)
                                    };
                   
                    default:
                        return new string[] {
                            "(Not implemented filter)"
                        };
                }
            }
        }
        
        public IEnumerable<String> Filters
        {
            get { return _filters; }
        }
        public int SelectedFilter
        {
            get { return this._selectedFilter; }
            set
            {
                this._selectedFilter = value;
                if (PropertyChanged != null)
                    PropertyChanged(this,
                        new PropertyChangedEventArgs("FilteredList")
                    );
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public partial class Order
    {
        private Model.EntrepriseEntities _context;
        public Order(Model.EntrepriseEntities context)
        {
            _context = context;
        }
        public decimal _Amount
        {
            get 
            {
                return from order in _context.Orders
                       join orders in _context.Order_Details on order.Order_ID equals orders.Order_ID
                       into orders 
                       group order by order.Order_ID into grp
                       select new
                       {
                           Orders = grp.Key,
                           Amount = grp.Sum(order => order.Freight)

                       };
            }
        }

    }
}
