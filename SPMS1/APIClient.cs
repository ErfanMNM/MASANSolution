using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPMS1
{
    public partial class APIClient : Component
    {
        public APIClient()
        {
            InitializeComponent();
        }

        public APIClient(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
