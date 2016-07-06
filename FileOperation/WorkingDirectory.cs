﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace PTL.FileOperation
{
    public class WorkingDirectory : INotifyPropertyChanged
    {
        private string _FolderName;
        public string FolderName
        {
            get { return _FolderName; }
            set
            {
                if (value != _FolderName)
                {
                    _FolderName = value;
                    UpdateFullDirectory();
                    NotifyPropertyChanged(nameof(FolderName));
                }
            }
        }


        public WorkingDirectory Parent { get; set; }
        public List<WorkingDirectory> SubDirectories { get; set; } = new List<WorkingDirectory>();
        private string _FullDirectory;
        public string FullDirectory
        {
            get { return _FullDirectory; }
            set
            {
                if (value != _FullDirectory)
                {
                    _FullDirectory = value;
                    NotifyPropertyChanged(nameof(FullDirectory));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private String SeekUp(string existedDirectory)
        {
            string newDir = FolderName + "\\" + existedDirectory;
            if (Parent != null)
                return Parent.SeekUp(newDir);
            else
                return newDir;
        }

        private void UpdateFullDirectory()
        {
            FullDirectory = SeekUp(FolderName);
            foreach (var item in SubDirectories)
            {
                item.UpdateFullDirectory();
            }
        }

        public void AddSubDirectories(params WorkingDirectory[] directories)
        {
            SubDirectories.AddRange(directories);
            foreach (var item in directories)
            {
                item.Parent = this;
                item.UpdateFullDirectory();
            }
        }

        public void AddSubDirectories(params IHaveWorkingDirectory[] directories)
        {
            List<WorkingDirectory> list = directories.ToList().ConvertAll<WorkingDirectory>((o) => o.Directory);
            SubDirectories.AddRange(list);
            foreach (var item in list)
            {
                item.Parent = this;
                item.UpdateFullDirectory();
            }
        }

        public void RemoveSubDirectories(params WorkingDirectory[] directories)
        {
            foreach (var item in directories)
            {
                if (SubDirectories.Contains(item))
                {
                    SubDirectories.Remove(item);
                    item.Parent = null;
                    item.UpdateFullDirectory();
                }
            }
        }

        public void RemoveSubDirectories(params IHaveWorkingDirectory[] directories)
        {
            List<WorkingDirectory> list = directories.ToList().ConvertAll<WorkingDirectory>((o) => o.Directory);
            foreach (var item in list)
            {
                if (SubDirectories.Contains(item))
                {
                    SubDirectories.Remove(item);
                    item.Parent = null;
                    item.UpdateFullDirectory();
                }
            }
        }

        public void CreateDirectory()
        {
            string[] folders = FullDirectory.Split('\\');
            string currentPath = folders[0];
            for (int i = 0; i < folders.Length; i++)
            {
                if (!System.IO.Directory.Exists(currentPath))
                    System.IO.Directory.CreateDirectory(currentPath);
            }

            foreach (var item in SubDirectories)
            {
                item.CreateDirectory();
            }
        }

        public override string ToString()
        {
            return FullDirectory;
        }
    }
}
