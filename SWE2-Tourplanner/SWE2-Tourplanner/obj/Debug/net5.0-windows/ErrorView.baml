   M S B A M L   `   `   `  ����K  GSWE2-Tourplanner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:SWE2_TourplannerSWE2_Tourplanner  5   	        SWE2_Tourplanner.ErrorView   5   	   \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:SWE2_Tourplanner     5   	     � Title$  	ErrorView��5   	    � SizeToContent$ Height=�6   $	��300��62   .��5	      � $��10q�6    � RowDefinitions 5
   
   �� 5      $��4* �6   �� 5      $��1* �6   .+�5   
   � $��0��6    5      �� 5      $
��Auto �6    �� 5      $
��Auto �6    .+�5      ��  �� Orientation$ Vertical=�6   $��0��60   .+�5      �� $��16(�67   $
��Wrap=�6E   
���\\����6   �� *	ErrorType+6Y   �� 5      $��5��6   5      �� 5      $
��Wrap=�6>   $��2��6R   ��6   �� *ErrorHandlingMessage+6R   5      �� 5   
   $��1��62   $��Ok��6?   $��0 5 0 0q�6L   ��6   �� *CloseCommand+6L   5      5      