using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocServices.Constants
{
    public class CommonMethods
    {
        public string GetDashboardStatus(int requestStatus)
        {
            int dashboardStatus = 0;

            switch (requestStatus)
            {
                case 1: 
                    dashboardStatus = 1;
                    break;

                case 2:
                    dashboardStatus = 2;
                    break;

                case 4:
                case 5:
                    dashboardStatus = 3;
                    break;

                case 6:
                    dashboardStatus = 4;
                    break;

                case 3:
                case 7:
                case 8:
                    dashboardStatus = 5;
                    break;

                case 9:
                    dashboardStatus = 6;
                    break;

                case 10:
                    dashboardStatus = 7;
                    break;

                case 11:
                    dashboardStatus = 8;
                    break;
            }

            if(dashboardStatus  == 0)
            {
                return string.Empty;
            }

            return ((DashboardRequestStatus)dashboardStatus).ToString();
        }

        public int[] GetRequestStatus(int dashboardStatus)
        {
            int[] myarray = new int[3];
            switch (dashboardStatus)
            {
                case 1:
                    myarray[0] = 1;
                    break;

                case 2:
                    myarray[0] = 2;
                    break;

                case 3:
                    myarray[0] = 4;
                    myarray[1] = 5;
                    break;

                case 4:
                    myarray[0] = 6;
                    break;

                case 5:
                    myarray[0] = 3;
                    myarray[1] = 7;
                    myarray[2] = 8;
                    break;

                case 6:
                    myarray[0] = 9;
                    break;

                case 7:
                    myarray[0] = 10;
                    break;

                case 8:
                    myarray[0] = 11;
                    break;
            }

            return myarray;
        }
    }
}
