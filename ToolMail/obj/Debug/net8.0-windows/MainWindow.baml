   M S B A M L   `   `   `  ����C  ?ToolMail, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:ToolMailToolMail  5   	       ToolMail.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:ToolMail     5   	     � Title$  
MainWindow��5   	   $	��600��6   $
��1000��6)   .��5	      � $��10q�6    � RowDefinitions 5
   
   �� 5      $��* �6   6)   �� 5      $
��auto �6   6,   .+�5   
   � $��0��6   $��	 0 0 0 10q�6    5      �� 5      $
��auto �6    60   �� 5      $��* �6    6-   �� 5      $
��auto �6    60   .+�5      �� $�� 0 0 0 5q�6   $��0��6+   $��Danh sách tài khoản��68   6U   �� 5      $
��True��5      $��1��5      $
��Auto=�6   .��5      &   System.Windows.Controls.DataGrid  -   5        $��accountGrid��5         AutoGenerateColumns$ False��5         
IsReadOnly$
 True��5      ��5      �� *	inputMail+ � ContextMenu 5      �� 5       .@�5!      m� -   5        $��Chọn tất cả��5!   '   6<   m� 5"      -   5        $��Bỏ chọn tất cả��5"   '   6?   m� 5#      -   5        $!��Nhập tài khoản (*.txt)��5#   '   6G   m� 5$      -   5        $!��Xuất tài khoản (*.txt)��5$   '   6G   m� 5%      -   5        $��Xóa tài khoản��5%   '   6>   5'         Columns 5)      4  .System.Windows.Controls.DataGridCheckBoxColumn  5*      ,  &System.Windows.Controls.DataGridColumn   
IsReadOnly$ False��62      Header$ Checked��6E      Width5  /System.Windows.Controls.DataGridLengthConverter$ 50 6   1  +System.Windows.Controls.DataGridBoundColumn	   Binding	 6V   �� *	Checked+
 �� Mode$
 TwoWay=�6   0  *System.Windows.Controls.DataGridTextColumn  5+      $	 STT��6-   $ 60 6R   	 6:   �� *STT+6R     5-      $ Maill��6.   $	 150 6W   	 6=   �� *Email+6W     5.      $
 Pass��6-   $	 150 6X   	 6;   �� *
Password+6X     5/      $ Proxy��6-   $	 200 6W   	 6=   �� *Proxy+6W     50      $ Trạng thái��6-   $ * 6\   	 6A   �� *Status+6\   52      53      5<      �� 5=   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��Center=�6;   .+�5>      �� -   5            MaterialDesignOutlinedButton#��[  5>      $��Bắt đầu��6i   6}   �� 5@      -   5        #��[  5@      ���#ffba00���#ffba00$��Cài đặt��6v   6�   5A      5B      5C      