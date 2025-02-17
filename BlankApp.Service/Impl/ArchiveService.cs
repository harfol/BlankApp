﻿using BlankApp.Service.Extensions;
using BlankApp.Service.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace BlankApp.Service.Impl
{
    public class ArchiveService : IArchiveService
    {
        private readonly IConfigurationService _cs;
        private readonly IArticleService _as;

        public ArchiveService(IConfigurationService configurationService, IArticleService articleService)
        {
            this._cs = configurationService;
            this._as = articleService;
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);
        }

        public bool IsArchiveDirectory(string str)
        {
            return Regex.IsMatch(str, @"^\d{4}?") || Regex.IsMatch(Path.GetFileName(str), @"^\d{4}?");
        }

        /*
                #region 档案配置文件操作
                public string GenerateXML(IList<FileDescription> fileDescriptionList)
                {
                    XmlDocument xmlDoc = new XmlDocument();

                    //创建Xml声明部分，即<?xml version="1.0" encoding="utf-8" ?>
                    xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");

                    //创建根节点
                    XmlNode rootNode = xmlDoc.CreateElement("Root");
                    //创建student子节点
                    XmlNode dirInfosNode = xmlDoc.CreateElement("DirInfos");
                    rootNode.AppendChild(dirInfosNode);
                    xmlDoc.AppendChild(rootNode);


                    foreach (FileDescription file in fileDescriptionList)
                    {
                        XmlElement xmlElement = xmlDoc.CreateElement("DirInfo");
                        xmlElement.SetAttribute("Title", file.FileName);
                        xmlElement.SetAttribute("Id", file.Id.ToString());

                        foreach (FileAttribute attr in file.Attributes)
                        {
                            XmlElement xe = xmlDoc.CreateElement(attr.Name);
                            xe.InnerText = FileAttributeUtil.GetValue(attr);
                            xmlElement.AppendChild(xe);
                        }
                        dirInfosNode.AppendChild(xmlElement);

                    }

                    string dst = Path.Combine(this.ArchivesPath, ArchivesInfoFileName);
                    xmlDoc.Save(dst);
                    return dst;
                }

                public FileDescription[] ReadXml()
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    List<FileDescription> files = new List<FileDescription>();
                    xmlDoc.Load(Path.Combine(ArchivesPath, ArchivesInfoFileName));
                    XmlNodeList xmlNodeList = xmlDoc.SelectNodes("//DirInfos/DirInfo");
                    DirectoryDescription[] directoryDescriptions = this._configurationService.GetDirectoryDescriptions();
                    foreach (XmlNode node in xmlNodeList)
                    {
                        FileDescription file = new FileDescription();
                        file.Attributes = new List<FileAttribute>();
                        file.FileName = node.Attributes["Title"]?.Value;
                        file.Id = int.Parse(node.Attributes["Id"]?.Value);
                        DirectoryDescription dd = directoryDescriptions.Where(d => d.DirectoryName.Equals(file.FileName)).First();
                        foreach (XmlNode n in node.ChildNodes)
                        {
                            FileAttribute fileAttribute = new FileAttribute();
                            fileAttribute.Name = n.Name;

                            string format = FileAttributeUtil.GetSeparatorChar().ToString();
                            for (int i = 0; i < dd.Attributes.Length; i++)
                            {
                                if (dd.Attributes[i].Equals(n.Name))
                                {
                                    format = dd.Formats[i];
                                    break;
                                }

                            }
                            fileAttribute.AttributeValue = FileAttributeUtil.GetAttributeValue(n.InnerText, format);
                            file.Attributes.Add(fileAttribute);
                        }
                        files.Add(file);
                    }

                    return files.ToArray();
                }
                #endregion*/




        #region Read()
        private bool ArticleMain(Article a)
        {
            return a.Type == ArticleTypes.Normal
                && !string.IsNullOrEmpty(a.Detail.Dossier)
                && !string.IsNullOrEmpty(a.Detail.Title)
                && a.Detail.Title.Contains("协议书")
                || a.Detail.Title.Contains("合同");
        }
        private int ArticleScan(Article a)
        {
            return (a.Type == ArticleTypes.Normal && !string.IsNullOrEmpty(a.Detail.Pages))
                ? int.Parse(a.Detail.Pages) : 0;
        }
        private int ArticleSummary(Article a)
        {
            return (a.Type == ArticleTypes.Normal && !string.IsNullOrEmpty(a.Detail.Copies) && !string.IsNullOrEmpty(a.Detail.Pages))
                ? int.Parse(a.Detail.Pages) * int.Parse(a.Detail.Copies) : 0;
        }
        private Detail ExtractSummary(ObservableCollection<Article> nodes)
        {
            Detail detail = new Detail();
            // 找到协议书
            detail = nodes.Where(ArticleMain).FirstOrDefault()?.Detail.Clone() ?? new Detail();
            
            detail.Copies = nodes.Sum(ArticleSummary).ToString();
            detail.Pages = nodes.Sum(ArticleScan).ToString();
            detail.PageNumber = "";
            detail.Title = "";
            return detail;
        }
        private Archive ExtractIdentifier(string archivesPath)
        {
            string name = Path.GetFileNameWithoutExtension(archivesPath);
            Archive archives = new Archive();
            archives.Id = int.Parse(name.Substring(0, 4));
            archives.Mask = name.Substring(5);
            archives.SourceFilePath = archivesPath;
            return archives;
        }
        public Archive Read(string archivesPath)
        {
            AutoResetEvent myEvent = new AutoResetEvent(false);
            string[] paths = Directory.GetDirectories(archivesPath, "*", SearchOption.TopDirectoryOnly);
            Archive archives = ExtractIdentifier(archivesPath);
            archives.Nodes = new ObservableCollection<Article>();
            
            int count = paths.Length;
            
            foreach (string path in paths)
            {

               ThreadPool.QueueUserWorkItem(
                    new WaitCallback((obj) =>
                    {
                        Article[] article = _as.Read(obj.ToString());
                        lock (this) 
                        {
                            archives.Nodes.AddRange(article);
                            if (--count <= 0)
                            {
                                myEvent.Set();
                            }
                        }
                        
                    }), path);
            }
            myEvent.WaitOne();
            
            archives.Detail = ExtractSummary(archives.Nodes);


            return archives;
        }
        #endregion
    }
}
