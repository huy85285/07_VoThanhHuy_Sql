using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _07_VoThanhHuy_Sql
{
    public partial class Form1 : Form
    {
        QLSV ds = new QLSV();
        QLSVTableAdapters.SINHVIENTableAdapter dapSV = new QLSVTableAdapters.SINHVIENTableAdapter();
        QLSVTableAdapters.KETQUATableAdapter dapKetQua = new QLSVTableAdapters.KETQUATableAdapter();
        QLSVTableAdapters.KHOATableAdapter dapKH = new QLSVTableAdapters.KHOATableAdapter();
        BindingSource bs = new BindingSource();
        public Form1()
        {
            InitializeComponent();
            bs.CurrentChanged += Bs_CurrentChanged;
        }

        private void Bs_CurrentChanged(object sender, EventArgs e)
        {
            lblstt.Text = bs.Position + 1 + $"/{bs.Count}";
            txttongdiem.Text = tinhDiem(((bs.Current as DataRowView).Row as QLSV.SINHVIENRow).MaSV).ToString();
            txtmasv.ReadOnly = true;
        }
        private Double tinhDiem(string MaSV)
        {
            var kq=ds.KETQUA.Compute("Sum(Diem)", $"MaSV='{MaSV}'");
            if (kq!=DBNull.Value)
            {
                return Convert.ToDouble(kq);
            }
            else
            {
                return 0;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadDuLieu();
            napcombo();
            napbinding();
        }


        private void loadDuLieu()
        {
            dapSV.Fill(ds.SINHVIEN);
            dapKH.Fill(ds.KHOA);
            dapKetQua.Fill(ds.KETQUA);
            bs.DataSource = ds.SINHVIEN;
        }
        private void napcombo()
        {
            cbokhoa.DataSource = ds.KHOA;
            cbokhoa.DisplayMember = "TenKH";
            cbokhoa.ValueMember = "MaKh";
        }
        private void napbinding()
        {
            foreach (Control item in this.Controls)
            {
                if (item.Name!= "txttongdiem" && item is TextBox)
                {
                    item.DataBindings.Add("text", bs, item.Name.Substring(3));
                }
                else if (item is DateTimePicker)
                {
                    item.DataBindings.Add("value", bs, item.Name.Substring(3));
                }
                else if (item is ComboBox)
                {
                    item.DataBindings.Add("SelectedValue", bs, "MaKH");
                }
                else if (item == chkphai)
                {
                    item.DataBindings.Add("Checked", bs, item.Name.Substring(3));
                }
            }
            txthocbong.DataBindings[0].FormatString = "#.## Đồng";
        }

        private void btnsau_Click(object sender, EventArgs e)
        {
            bs.MoveNext();
        }

        private void btntruoc_Click(object sender, EventArgs e)
        {
            bs.MovePrevious();
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            bs.AddNew();
            txtmasv.ReadOnly = false;
        }

        private void btnkhong_Click(object sender, EventArgs e)
        {
            bs.CancelEdit();
        }

        private void btnhuy_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xác nhận muốn xoá","Thông báo",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)==DialogResult.OK)
            {
                bs.RemoveCurrent();
                if (dapSV.Update(ds)>0)
                {
                    MessageBox.Show("Đã xoá thành công");
                }
            }
            else
            {
                return;
            }
        }

        private void btnghi_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn tạo mới", "Thông báo",MessageBoxButtons.YesNo)==DialogResult.Yes)
            {
                if (!txtmasv.ReadOnly)
                {
                    if (ds.SINHVIEN.FindByMaSV(txtmasv.Text)!=null)
                    {
                        MessageBox.Show("Đã trùng khoá chính");
                        return;
                    }
                }
                bs.EndEdit();
                int count = dapSV.Update(ds.SINHVIEN);
                if (count > 0)
                {
                    MessageBox.Show("Đã thay đổi", "Thông báo");
                }
            }
        }
    }
}
