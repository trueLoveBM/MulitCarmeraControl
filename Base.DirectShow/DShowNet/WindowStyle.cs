using System;
using System.Collections.Generic;
using System.Text;

namespace Base.DirectShow
{
    [Flags]
    public enum WindowStyle
    {
        // Token: 0x0400074B RID: 1867
        Overlapped = 0,
        // Token: 0x0400074C RID: 1868
        Popup = -2147483648,
        // Token: 0x0400074D RID: 1869
        Child = 1073741824,
        // Token: 0x0400074E RID: 1870
        Minimize = 536870912,
        // Token: 0x0400074F RID: 1871
        Visible = 268435456,
        // Token: 0x04000750 RID: 1872
        Disabled = 134217728,
        // Token: 0x04000751 RID: 1873
        ClipSiblings = 67108864,
        // Token: 0x04000752 RID: 1874
        ClipChildren = 33554432,
        // Token: 0x04000753 RID: 1875
        Maximize = 16777216,
        // Token: 0x04000754 RID: 1876
        Caption = 12582912,
        // Token: 0x04000755 RID: 1877
        Border = 8388608,
        // Token: 0x04000756 RID: 1878
        DlgFrame = 4194304,
        // Token: 0x04000757 RID: 1879
        VScroll = 2097152,
        // Token: 0x04000758 RID: 1880
        HScroll = 1048576,
        // Token: 0x04000759 RID: 1881
        SysMenu = 524288,
        // Token: 0x0400075A RID: 1882
        ThickFrame = 262144,
        // Token: 0x0400075B RID: 1883
        Group = 131072,
        // Token: 0x0400075C RID: 1884
        TabStop = 65536,
        // Token: 0x0400075D RID: 1885
        MinimizeBox = 131072,
        // Token: 0x0400075E RID: 1886
        MaximizeBox = 65536
    }
}
