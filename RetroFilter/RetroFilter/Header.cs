using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace RetroFilter
{
    [Serializable]
    public class Header : INotifyPropertyChanged
    {
        private string _name = "No Name";

        [XmlElement(ElementName = "name")]
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private string _description = "No Description";

        [XmlElement(ElementName = "description")]
        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        [XmlElement(ElementName = "category")]
        public string Category { get; set; } = string.Empty;

        [XmlElement(ElementName = "version")]
        public string Version { get; set; } = string.Empty;

        [XmlElement(ElementName = "date")]
        public string Date { get; set; } = string.Empty;

        [XmlElement(ElementName = "author")]
        public string Author { get; set; } = string.Empty;

        [XmlElement(ElementName = "email")]
        public string Email { get; set; } = string.Empty;

        [XmlElement(ElementName = "homepage")]
        public string Homepage { get; set; } = string.Empty;

        [XmlElement(ElementName = "url")]
        public string Url { get; set; } = string.Empty;

        [XmlElement(ElementName = "comment")]
        public string Comment { get; set; } = string.Empty;

        [XmlElement(ElementName = "clrmamepro")]
        public ClrMamePro ClrMamePro { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    [Serializable]
    public class ClrMamePro
    {
        [XmlAttribute(AttributeName = "forcenodump")]
        public string ForceNoDump { get; set; } = string.Empty;
    }
}