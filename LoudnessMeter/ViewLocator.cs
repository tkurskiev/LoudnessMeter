using Avalonia.Controls;
using Avalonia.Controls.Templates;
using LoudnessMeter.ViewModels;
using System;

namespace LoudnessMeter
{
    public class ViewLocator : IDataTemplate
    {
        public IControl? Build(object? data)
        {
            if (data is null)
                return null;

            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = name };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}