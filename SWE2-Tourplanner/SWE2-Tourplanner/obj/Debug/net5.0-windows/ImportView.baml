   M S B A M L   `   `   `  ����K  GSWE2-Tourplanner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:SWE2_TourplannerSWE2_Tourplanner  5   	   !    SWE2_Tourplanner.ImportView   5   	   \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:SWE2_Tourplanner     5   	     � Title$  
ImportView��5   	   $	��200��6   $	��300��6)   .��5	      �  � RowDefinitions 5
   
   �� 5      $��1* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��64   $��5q�6A   ��6   �� *	JsonPaths+5      �� 5   
   $��1��6   $��Search for files��6   $��5q�6`   ��69   �� *SelectPathsCommand+5      � 5   
   $��2��6   $��5q�6    � ColumnDefinitions 5      �� 5      $��1* �6#   �� 5      $��1* �6#   .+�5      �� $��0��6   $
��Exit��6%   ��64   �� *ExitCommand+6V   �� 5      $��1��6   $��Import files��6%   ��6<   �� *ConfirmImportCommand+6g   5      5      5      