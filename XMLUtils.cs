using System;
using System.Data;
using System.IO;
using System.Xml.Schema;
using System.Xml.Linq;

namespace Capstone_Game_Platform
{
    internal class XMLUtils
    {
        /// <summary>
        /// Path to XML File
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Create new default XML File
        /// </summary>
        /// <param name="path"></param>
        /// <returns>bool - true if file created, false if file exists</returns>
        public bool CreateXMLfile()
        {
            if (!File.Exists(FilePath))
            {
                try
                {
                    XDocument doc = XDocument.Parse(Properties.Resources.Cloud9DataXML);
                    doc.Save(FilePath);
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error occured when trying to create default XML File at: " + FilePath, ex);
                }
            }
            return false;
        }

        /// <summary>
        /// Read XML File. If file is not found, default XML file is created and read.
        /// </summary>
        /// <returns>DataSet</returns>
        public DataSet ReadXMLfile()
        {
            if (!File.Exists(FilePath))
            {
                CreateXMLfile();
            }

            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(FilePath);
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception("Can not access XML File at: " + FilePath, ex);
            }
        }

        /// <summary>
        /// Save Data Set to XML file
        /// </summary>
        /// <param name="ds">Data Set</param>
        /// <returns>bool - true if file writen</returns>
        public bool UpdateXMLfile(DataSet ds)
        {
            try
            {
                ds.AcceptChanges(); 
                ds.WriteXml(FilePath);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Can not access XML File at: " + FilePath, ex);
            }
        }

        /// <summary>
        /// Deletes XML File
        /// </summary>
        /// <returns>bool - true if file deleted/ false if there was no file to delete</returns>
        public bool DeleteXMLfile()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    File.Delete(FilePath);
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error occured when trying to Delete XML File at: " + FilePath, ex);
                }
            } 
            return false;
        }

        // <summary>
        // Validates file, if no file exsists, file is created then validated
        // </summary>
        // <returns>Bool - returns true if file is validated</returns>
        public bool ValidateXmlFile()
        {
            if (!File.Exists(FilePath))
            {
                CreateXMLfile();
            }

            bool isXmlValid = true;
            XDocument xSchema = XDocument.Parse(Properties.Resources.AppXSD);
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(Properties.Resources.AppURN, xSchema.CreateReader());
            XDocument xmlDocument = XDocument.Load(FilePath);
            xmlDocument.Validate(schemas, validationEventHandler: (o, e) => { isXmlValid = false; });
            return isXmlValid;
        }
    }
}

