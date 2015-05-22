﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.34014
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.33440.
// 
namespace MazeNetClient.XSDGenerated {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class MazeCom {
        
        private object itemField;
        
        private MazeComType mcTypeField;
        
        private int idField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AcceptMessage", typeof(AcceptMessageType))]
        [System.Xml.Serialization.XmlElementAttribute("AwaitMoveMessage", typeof(AwaitMoveMessageType))]
        [System.Xml.Serialization.XmlElementAttribute("DisconnectMessage", typeof(DisconnectMessageType))]
        [System.Xml.Serialization.XmlElementAttribute("LoginMessage", typeof(LoginMessageType))]
        [System.Xml.Serialization.XmlElementAttribute("LoginReplyMessage", typeof(LoginReplyMessageType))]
        [System.Xml.Serialization.XmlElementAttribute("MoveMessage", typeof(MoveMessageType))]
        [System.Xml.Serialization.XmlElementAttribute("WinMessage", typeof(WinMessageType))]
        public object Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public MazeComType mcType {
            get {
                return this.mcTypeField;
            }
            set {
                this.mcTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AcceptMessageType {
        
        private bool acceptField;
        
        private ErrorType errorCodeField;
        
        /// <remarks/>
        public bool accept {
            get {
                return this.acceptField;
            }
            set {
                this.acceptField = value;
            }
        }
        
        /// <remarks/>
        public ErrorType errorCode {
            get {
                return this.errorCodeField;
            }
            set {
                this.errorCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    public enum ErrorType {
        
        /// <remarks/>
        NOERROR,
        
        /// <remarks/>
        ERROR,
        
        /// <remarks/>
        AWAIT_LOGIN,
        
        /// <remarks/>
        AWAIT_MOVE,
        
        /// <remarks/>
        ILLEGAL_MOVE,
        
        /// <remarks/>
        TIMEOUT,
        
        /// <remarks/>
        TOO_MANY_TRIES,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DisconnectMessageType {
        
        private string nameField;
        
        private ErrorType erroCodeField;
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        public ErrorType erroCode {
            get {
                return this.erroCodeField;
            }
            set {
                this.erroCodeField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class WinMessageType {
        
        private boardType boardField;
        
        private WinMessageTypeWinner winnerField;
        
        /// <remarks/>
        public boardType board {
            get {
                return this.boardField;
            }
            set {
                this.boardField = value;
            }
        }
        
        /// <remarks/>
        public WinMessageTypeWinner winner {
            get {
                return this.winnerField;
            }
            set {
                this.winnerField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class boardType {
        
        private boardTypeRow[] rowField;
        
        private cardType shiftCardField;
        
        private positionType forbiddenField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("row")]
        public boardTypeRow[] row {
            get {
                return this.rowField;
            }
            set {
                this.rowField = value;
            }
        }
        
        /// <remarks/>
        public cardType shiftCard {
            get {
                return this.shiftCardField;
            }
            set {
                this.shiftCardField = value;
            }
        }
        
        /// <remarks/>
        public positionType forbidden {
            get {
                return this.forbiddenField;
            }
            set {
                this.forbiddenField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class boardTypeRow {
        
        private cardType[] colField;
        
        private string optionalAttirbuteField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("col")]
        public cardType[] col {
            get {
                return this.colField;
            }
            set {
                this.colField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OptionalAttirbute {
            get {
                return this.optionalAttirbuteField;
            }
            set {
                this.optionalAttirbuteField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class cardType {
        
        private cardTypeOpenings openingsField;
        
        private int[] pinField;
        
        private treasureType treasureField;
        
        private bool treasureFieldSpecified;
        
        /// <remarks/>
        public cardTypeOpenings openings {
            get {
                return this.openingsField;
            }
            set {
                this.openingsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("playerID", IsNullable=false)]
        public int[] pin {
            get {
                return this.pinField;
            }
            set {
                this.pinField = value;
            }
        }
        
        /// <remarks/>
        public treasureType treasure {
            get {
                return this.treasureField;
            }
            set {
                this.treasureField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool treasureSpecified {
            get {
                return this.treasureFieldSpecified;
            }
            set {
                this.treasureFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class cardTypeOpenings {
        
        private bool topField;
        
        private bool bottomField;
        
        private bool leftField;
        
        private bool rightField;
        
        /// <remarks/>
        public bool top {
            get {
                return this.topField;
            }
            set {
                this.topField = value;
            }
        }
        
        /// <remarks/>
        public bool bottom {
            get {
                return this.bottomField;
            }
            set {
                this.bottomField = value;
            }
        }
        
        /// <remarks/>
        public bool left {
            get {
                return this.leftField;
            }
            set {
                this.leftField = value;
            }
        }
        
        /// <remarks/>
        public bool right {
            get {
                return this.rightField;
            }
            set {
                this.rightField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    public enum treasureType {
        
        /// <remarks/>
        Start01,
        
        /// <remarks/>
        Start02,
        
        /// <remarks/>
        Start03,
        
        /// <remarks/>
        Start04,
        
        /// <remarks/>
        sym01,
        
        /// <remarks/>
        sym02,
        
        /// <remarks/>
        sym03,
        
        /// <remarks/>
        sym04,
        
        /// <remarks/>
        sym05,
        
        /// <remarks/>
        sym06,
        
        /// <remarks/>
        sym07,
        
        /// <remarks/>
        sym08,
        
        /// <remarks/>
        sym09,
        
        /// <remarks/>
        sym10,
        
        /// <remarks/>
        sym11,
        
        /// <remarks/>
        sym12,
        
        /// <remarks/>
        sym13,
        
        /// <remarks/>
        sym14,
        
        /// <remarks/>
        sym15,
        
        /// <remarks/>
        sym16,
        
        /// <remarks/>
        sym17,
        
        /// <remarks/>
        sym18,
        
        /// <remarks/>
        sym19,
        
        /// <remarks/>
        sym20,
        
        /// <remarks/>
        sym21,
        
        /// <remarks/>
        sym22,
        
        /// <remarks/>
        sym23,
        
        /// <remarks/>
        sym24,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class positionType {
        
        private int rowField;
        
        private int colField;
        
        /// <remarks/>
        public int row {
            get {
                return this.rowField;
            }
            set {
                this.rowField = value;
            }
        }
        
        /// <remarks/>
        public int col {
            get {
                return this.colField;
            }
            set {
                this.colField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class WinMessageTypeWinner {
        
        private int idField;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class MoveMessageType {
        
        private positionType shiftPositionField;
        
        private positionType newPinPosField;
        
        private cardType shiftCardField;
        
        /// <remarks/>
        public positionType shiftPosition {
            get {
                return this.shiftPositionField;
            }
            set {
                this.shiftPositionField = value;
            }
        }
        
        /// <remarks/>
        public positionType newPinPos {
            get {
                return this.newPinPosField;
            }
            set {
                this.newPinPosField = value;
            }
        }
        
        /// <remarks/>
        public cardType shiftCard {
            get {
                return this.shiftCardField;
            }
            set {
                this.shiftCardField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class TreasuresToGoType {
        
        private int playerField;
        
        private int treasuresField;
        
        /// <remarks/>
        public int player {
            get {
                return this.playerField;
            }
            set {
                this.playerField = value;
            }
        }
        
        /// <remarks/>
        public int treasures {
            get {
                return this.treasuresField;
            }
            set {
                this.treasuresField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AwaitMoveMessageType {
        
        private boardType boardField;
        
        private TreasuresToGoType[] treasuresToGoField;
        
        private treasureType treasureField;
        
        /// <remarks/>
        public boardType board {
            get {
                return this.boardField;
            }
            set {
                this.boardField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("treasuresToGo")]
        public TreasuresToGoType[] treasuresToGo {
            get {
                return this.treasuresToGoField;
            }
            set {
                this.treasuresToGoField = value;
            }
        }
        
        /// <remarks/>
        public treasureType treasure {
            get {
                return this.treasureField;
            }
            set {
                this.treasureField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class LoginReplyMessageType {
        
        private int newIDField;
        
        /// <remarks/>
        public int newID {
            get {
                return this.newIDField;
            }
            set {
                this.newIDField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class LoginMessageType {
        
        private string nameField;
        
        /// <remarks/>
        public string name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    public enum MazeComType {
        
        /// <remarks/>
        LOGIN,
        
        /// <remarks/>
        LOGINREPLY,
        
        /// <remarks/>
        AWAITMOVE,
        
        /// <remarks/>
        MOVE,
        
        /// <remarks/>
        ACCEPT,
        
        /// <remarks/>
        WIN,
        
        /// <remarks/>
        DISCONNECT,
    }
}
