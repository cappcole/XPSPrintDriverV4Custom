using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EclaimsPrint
{
    public class DataGridViewEx : DataGridView
    {

        private Image _backgroundPic;



        [Browsable(true)]

        public override Image BackgroundImage

        {

            get { return _backgroundPic; }

            set { _backgroundPic = value; }

        }



        protected override void PaintBackground(System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle gridBounds)

        {

            base.PaintBackground(graphics, clipBounds, gridBounds);



            if (((this.BackgroundImage != null)))

            {

                graphics.FillRectangle(Brushes.White, gridBounds);

                Point imageLocation = new Point();
                imageLocation.X = (this.Width+50) / 2 - this.BackgroundImage.Width / 2;
                imageLocation.Y = 10+this.Height / 2 - this.BackgroundImage.Height / 2;
                //graphics.DrawImage(this.BackgroundImage, gridBounds);
                graphics.DrawImage(this.BackgroundImage, imageLocation);
                SetCellsTransparent();
            }

        }


        //Make BackgroundImage can be seen in all cells

        public void SetCellsTransparent()

        {

            this.EnableHeadersVisualStyles = false;



            //this.ColumnHeadersDefaultCellStyle.BackColor = Color.Transparent;



            //this.RowHeadersDefaultCellStyle.BackColor = Color.Transparent;



            foreach (DataGridViewColumn col in this.Columns)

            {

                col.DefaultCellStyle.BackColor = Color.Transparent;


            }

        }

    }
}
