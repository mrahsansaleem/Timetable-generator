﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TimeTableGenerator.Template;

namespace TimeTableGenerator
{
    public partial class Time_Table : templateForm
    {
        private int maxRoomCap = -1;
        private string className = "";
        private int startTime = 0;
        private int endTime = 24;
        private int currentRoomNo = 1;
        private List<cClassData> classStore = new List<cClassData>();
        

        private void showInputClass()
        {
            DGVinputData.DataSource = "";
            DGVinputData.DataSource = classStore;
        }
        private void showOutputClass()
        {
            classStore = classStore.OrderBy(o => o.RoomNo).ToList();
            DGVoutputData.DataSource = "";
            DGVoutputData.DataSource = classStore;
        }
        private void sortByEndTime(List <cClassData> pClassStore)
        {
            for (int j= 0; j < pClassStore.Count; j++)
            {
                for (int i = j + 1; i < pClassStore.Count; i++)
                {
                    if (pClassStore[i].EndTime<=pClassStore[i-1].EndTime)
                    {
                        cClassData temp = pClassStore[i];
                        pClassStore[i] = pClassStore[i -1];
                        pClassStore[i-1] = temp;
                    }
                }
            }
        }

        private void GenerateOptimalSelectedClass(List<cClassData> pClassStore)
        {
            currentRoomNo = 1;
            sortByEndTime(pClassStore);
            while(currentRoomNo<maxRoomCap)
            {
                //select 1st non assigned room after sorting
                foreach (cClassData x in classStore)
                {
                    if (x.RoomNo == "no assign")
                    {
                        x.RoomNo = currentRoomNo.ToString();
                        break;
                    }
                }
                //^^^^select 1st non assigned room after sorting^^^^

                int i,j;
                i = 0;
                for (j = 1; j < pClassStore.Count; j++)
                {
                    if(pClassStore[j].StartTime>=pClassStore[i].EndTime && pClassStore[j].RoomNo == "no assign")
                    {
                        pClassStore[j].RoomNo = currentRoomNo.ToString();
                        i = j;
                    }
                }
                currentRoomNo++;
            }            
        }

        public Time_Table()
        {
            InitializeComponent();
        }
        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (classStore.Count > 0)
            {
                maxRoomCap = Convert.ToInt32(txtNumberofrooms.Text);//max rooms
                txtNumberofrooms.ReadOnly = true; //after 1st insertion make readonly can't be change later
                txtNumberofrooms.BackColor = Color.DimGray;
                //read data
                className = TBclassName.Text;
                startTime = Convert.ToInt32(CBstartTime.Text);
                endTime = Convert.ToInt32(CBendTime.Text);

                cClassData mydata = new cClassData();//create a object
                mydata.ClassName = className;
                mydata.StartTime = startTime;
                mydata.EndTime = endTime;
                //add new data object (new class) in to classStore
                classStore.Add(mydata);

                //reset GUI data input fields
                CBstartTime.Text = "";
                CBendTime.Text = "";
                TBclassName.Text = "";
                className = "";
                startTime = 0;
                endTime = 24;
                //whole data on datagrid view
                showInputClass();
            }
            else
            {
                MessageBox.Show("nothing to insert", "pleas enter the data and then process further", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DGVinputData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(classStore.Count > 0)
            {
                if(e.ColumnIndex==0)        //delete button on DGV
                {
                    classStore.RemoveAt(e.RowIndex);
                    showInputClass();
                }
                else if(e.ColumnIndex==1)   //edit button on DGV
                {
                    editForm nform = new editForm(classStore,e.RowIndex);
                    nform.Show();
                }
            }
            else
            {
                MessageBox.Show("nothing to delete or edit", "invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateOptimalSelectedClass(classStore);
            showOutputClass();
        }
    }
}
