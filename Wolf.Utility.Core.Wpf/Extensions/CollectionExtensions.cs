using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Wolf.Utility.Core.Wpf.Extensions
{
    public static  class CollectionExtensions
{
        /// <summary>
        /// Adds the specified amount of rows as simple 'new RowDefinition()' with Height set to 'new GridLength(0, GridUnitType.Auto)'
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="amount"></param>
        public static void AddAmount(this RowDefinitionCollection collection, int amount) 
        {
            for (int i = 0; i < amount; i++)
            {
                collection.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Auto) });
            }
        }

        /// <summary>
        /// Adds the specified amount of columns as simple 'new ColumnDefinition()' with Width set to 'new GridLength(0, GridUnitType.Auto)'
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="amount"></param>
        public static void AddAmount(this ColumnDefinitionCollection collection, int amount) 
        {
            for (int i = 0; i < amount; i++)
            {
                collection.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Auto) });
            }
        }
    }
}
