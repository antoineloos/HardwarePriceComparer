   M S B A M L   `   `   `  ����B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5   	       WpfApp1.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:WpfApp1     5   	     � WindowState$  	Maximized=�5	   	    � Title$ 
MainWindow��5
   	   $	��300��6   $	��300��6)    � DataContext 5      !   WpfApp1.MainWindowViewModel  5   
   -   5        datacontext��5   $   .��5      �  � RowDefinitions 5   
   �� 5      $
��auto �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   .+�5   
   �� $��./Assets/amazon.png��6   $��2��6-   $��10q�6=   �� 5   
   $��./Assets/ldlc.png��6   $��0��6+   $��10q�6;   �� 5   
   $��./Assets/topachat.png��6   $��1��6/   $��10q�6?   �� 5   
   $��./Assets/grosbill.png��6   $��3��6/   $��10q�6?   �� 5   
   $��./Assets/materielnet.png��6   $��4��62   $��10q�6B   &   System.Windows.Controls.DataGrid  5    
   -   5          Name$ dataGrdldlc��5    w   $��2q�6   
�������$��1��6:   $��0��6G   ��6W   �� *	LstLDLC+6w     5!   
   -   5        $ dataGrdtopachat��5!   {   $��2q�6   
�������$��1��6:   $��1��6G   ��6W   �� *LstTopAchat+6{     5"   
   -   5        $ dataGrdamazon��5"   y   $��2q�6   
�������$��1��6:   $��2��6G   ��6W   �� *	LstAmazon+6y     5#   
   -   5        $ dataGrdgrosbill��5#   {   $��2q�6   
�������$��1��6:   $��3��6G   ��6W   �� *LstGrosbill+6{     5$   
   -   5        $ dataGrdMaterielNet��5$   ~   $��2q�6   
�������$��1��6:   $��4��6G   ��6W   �� *LstMaterielNet+6~   5&      5'      