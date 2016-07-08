namespace LeagueSharp.Common
{
    using Newtonsoft.Json;

    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    ///     The menu settings manager (serialization, saving, etc.)
    /// </summary>
    [Serializable]
    internal static class SavedSettings
    {
        #region Static Fields

        /// <summary>
        ///     The loaded files collection.
        /// </summary>
        public static Dictionary<string, Dictionary<string, string>> LoadedFiles =
            new Dictionary<string, Dictionary<string, string>>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the saved data.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="key">
        ///     The key.
        /// </param>
        /// <returns>
        ///     The <see cref="byte" /> collection of the data.
        /// </returns>
        public static string GetSavedData(string name, string key)
        {
            var dic = LoadedFiles.ContainsKey(name) ? LoadedFiles[name] : Load(name);
            return dic == null ? null : dic.ContainsKey(key) ? dic[key] : null;
        }

        /// <summary>
        ///     Loads the specific entry.
        /// </summary>
        /// <param name="name">
        ///     The name of the entry.
        /// </param>
        /// <returns>
        ///     The <see cref="Dictionary{TKey,TValue}" /> collection of the entry contents.
        /// </returns>
        public static Dictionary<string, string> Load(string name)
        {
            try
            {
                var jsonFile = Path.Combine(MenuSettings.MenuConfigPath, name + ".json");
                var binFile = Path.Combine(MenuSettings.MenuConfigPath, name + ".bin");

                if (File.Exists(binFile))
                {
                    var deserialized = Utils.Deserialize<Dictionary<string, byte[]>>(File.ReadAllBytes(binFile));
                    var deserializedObj = new Dictionary<string, object>();
        
                    foreach (var keyValuePair in deserialized)
                    {
                        deserializedObj[keyValuePair.Key] = Utils.Deserialize<object>(keyValuePair.Value);
                    }

                    var deserializedJson = new Dictionary<string, string>();

                    foreach (var keyValuePair in deserializedObj)
                    {
                        deserializedJson[keyValuePair.Key] = JsonConvert.SerializeObject(keyValuePair.Value);
                    }

                    File.Delete(binFile);
                    return deserializedJson;
                }

                if (File.Exists(jsonFile))
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(jsonFile));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        /// <summary>
        ///     Saves the specificed entry.
        /// </summary>
        /// <param name="name">
        ///     The name of the entry.
        /// </param>
        /// <param name="entries">
        ///     The entries.
        /// </param>
        public static void Save(string name, Dictionary<string, string> entries)
        {
            try
            {
                Directory.CreateDirectory(MenuSettings.MenuConfigPath);
                File.WriteAllText(Path.Combine(MenuSettings.MenuConfigPath, name + ".json"), JsonConvert.SerializeObject(entries, Formatting.Indented));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion
    }
}