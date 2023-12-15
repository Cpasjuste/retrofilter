using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace RetroFilter.Sources
{
    [Serializable]
    public class Header : INotifyPropertyChanged
    {
        private string? _name;

        [XmlElement(ElementName = "name")]
        public string? Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        private string? _description;

        [XmlElement(ElementName = "description")]
        public string? Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        [XmlElement(ElementName = "category")]
        public string? Category { get; set; }

        [XmlElement(ElementName = "version")]
        public string? Version { get; set; }

        [XmlElement(ElementName = "date")]
        public string? Date { get; set; }

        [XmlElement(ElementName = "author")]
        public string? Author { get; set; }

        [XmlElement(ElementName = "email")]
        public string? Email { get; set; }

        [XmlElement(ElementName = "homepage")]
        public string? Homepage { get; set; }

        [XmlElement(ElementName = "url")]
        public string? Url { get; set; }

        [XmlElement(ElementName = "comment")]
        public string? Comment { get; set; }

        [XmlElement(ElementName = "clrmamepro")]
        public ClrMamePro? ClrMamePro { get; set; }

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
        public string? ForceNoDump { get; set; }
    }
}