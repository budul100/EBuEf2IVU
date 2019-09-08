using System;
using System.IO;
using System.Reflection;

namespace EBuEf2IVUCore.Services
{
    internal static class AppInfoService
    {
        #region Private Fields

        private static readonly Assembly assembly = Assembly.GetEntryAssembly();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// Gets the company information for the product.
        /// </summary>
        public static string Company => GetAttributeValue<AssemblyCompanyAttribute>(a => a.Company);

        /// <summary>
        /// Gets the copyright information for the product.
        /// </summary>
        public static string Copyright => GetAttributeValue<AssemblyCopyrightAttribute>(a => a.Copyright);

        /// <summary>
        /// Gets the description about the application.
        /// </summary>
        public static string Description => GetAttributeValue<AssemblyDescriptionAttribute>(a => a.Description);

        /// <summary>
        ///  Gets the product's full name.
        /// </summary>
        public static string Product => GetAttributeValue<AssemblyProductAttribute>(a => a.Product);

        /// <summary>
        /// Gets the title property
        /// </summary>
        public static string ProductTitle => GetAttributeValue<AssemblyTitleAttribute>(
            resolveFunc: a => a.Title,
            defaultResult: Path.GetFileNameWithoutExtension(assembly.CodeBase));

        /// <summary>
        /// Gets the application's version
        /// </summary>
        public static string VersionFull => assembly.GetName().Version?.ToString();

        /// <summary>
        /// Gets the application's version
        /// </summary>
        public static string VersionMajorMinor
        {
            get
            {
                var version = assembly.GetName().Version;

                return $"{version?.Major}.{version?.Minor}";
            }
        }

        #endregion Public Properties

        #region Private Methods

        private static string GetAttributeValue<TAttr>
            (Func<TAttr, string> resolveFunc, string defaultResult = null) where TAttr : Attribute
        {
            var attributes = assembly.GetCustomAttributes(typeof(TAttr), false);

            return
                attributes.Length > 0 ?
                resolveFunc?.Invoke((TAttr)attributes[0]) :
                defaultResult;
        }

        #endregion Private Methods
    }
}