using System;
using System.Collections.Generic;



namespace Mathematics
{
    public class Cylinder : Primitive
    {
        //  *****************************************************************************************
        //  DECLARATIONS
        //
        //  ************************************************************
        //  
        //  Identification
        private String _CylinderID = string.Empty;
        //
        //  Name
        private String _Name = string.Empty;
        //
        //  Data
        //
        //  Dimensions
        private Double _Height;
        private Double _TaperAngle; //  (Degrees)
        //
        //  Owned & Owning Objects
        //
        //  Base Circle
        private Circle _BaseCircle = new Circle();
        private Circle _TopCircle = new Circle();
        //  *****************************************************************************************


        //  ****************************************************************************************
        //  INITIALIZATIONS
        //
        //  ************************************************************

        //  *****************************************************************************************


        //  *****************************************************************************************
        //  ENUMERATIONS
        //
        //  ************************************************************

        //  *****************************************************************************************


        //  *****************************************************************************************
        //  CYLINDER CONSTRUCTOR
        //
        //  ************************************************************
        public Cylinder()
        {
            this.Is2D = false;
            this.ThreeDType = ThreeDPrimitiveTypeEnum.Cylinder;

            //
            //  Location
            this.CenterPoint = new Point();
            this.BaseCircle = new Circle();
            this.TopCircle = new Circle();
        }
        //  *****************************************************************************************


        //  *****************************************************************************************
        //  PROPERTIES
        //
        //  ************************************************************
        //  
        //  Identification
        public String CylinderID
        {
            set => _CylinderID = value;
            get
            {
                return _CylinderID;
            }
        }
        //
        //  Name
        public new String Name
        {
            set => _Name = value;
            get
            {
                return _Name;
            }
        }
        ///
        //  Data
        //
        //  Dimensions
        public Double Height
        {
            set => _Height = value;
            get
            {
                return _Height;
            }
        }
        public Double TaperAngle
        {
            set => _TaperAngle = value;
            get
            {
                return _TaperAngle;
            }
        }

        //
        //  Owned & Owning Objects
        //
        //  Base Circle
        public Circle BaseCircle
        {
            set => _BaseCircle = value;
            get
            {
                return _BaseCircle;
            }
        }
        public Circle TopCircle
        {
            set => _TopCircle = value;
            get
            {
                return _TopCircle;
            }
        }
        //  *****************************************************************************************


        //  *****************************************************************************************
        //  METHODS
        //
        //  ************************************************************

        //  *****************************************************************************************


        //  *****************************************************************************************
        //  EVENTS
        //
        //  ************************************************************

        //  *****************************************************************************************
    }
}
