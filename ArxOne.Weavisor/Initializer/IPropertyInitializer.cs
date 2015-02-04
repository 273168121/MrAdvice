﻿#region Weavisor
// Weavisor
// A simple post build weaving package
// https://github.com/ArxOne/Weavisor
// Release under MIT license http://opensource.org/licenses/mit-license.php
#endregion

namespace ArxOne.Weavisor.Initializer
{
    using System.Reflection;

    /// <summary>
    /// Runtime initialization for properties
    /// </summary>
    public interface IPropertyInitializer : IInitializer
    {
        /// <summary>
        /// Invoked once per property, when assembly is loaded
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        void Initialize(PropertyInfo propertyInfo);
    }
}