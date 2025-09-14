using CommunityToolkit.Mvvm.ComponentModel;
using OTILib.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using talk2.Models;
using talk2.Services;

namespace talk2.ViewModels
{
    public partial class DivViewModel : ObservableObject
    {
        private readonly IDivService _divService;
        public DivViewModel(IDivService divService)
        {
            _divService = divService;
        }

        public async Task InitAsync()
        {
            var divs = await _divService.getDivList();
            foreach(var d in divs)
            {
                Divs.Add(d);
            }
        }

        public ObservableCollection<Div> Divs { get; set; } = new();

        [ObservableProperty]
        private Div selectedDiv;

        [ObservableProperty]
        private Div newDiv = new();

        [CommunityToolkit.Mvvm.Input.RelayCommand]
        private async Task Select(int divNo)
        {
            foreach (var d in Divs)
            {
                if (d.DivNo == divNo)
                {
                    SelectedDiv = d;
                    break;
                }
            }
        }

        [CommunityToolkit.Mvvm.Input.RelayCommand]
        private async Task Create()
        {
            await _divService.InsertDiv(NewDiv.DivNm);
        }

        [CommunityToolkit.Mvvm.Input.RelayCommand]
        private async Task Edit()
        {
            await _divService.EditDiv(SelectedDiv.DivNo, SelectedDiv.DivNm);
        }
    }
}
