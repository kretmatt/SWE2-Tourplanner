   M S B A M L   `   `   `  ����K  GSWE2-Tourplanner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS.clr-namespace:SWE2_Tourplanner.ValidationRules SWE2_Tourplanner.ValidationRules  5   	   3clr-namespace:SWE2_TourplannerSWE2_Tourplanner  5   	   +    %SWE2_Tourplanner.CreateUpdateTourView   5   	   \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   E
Validators.clr-namespace:SWE2_Tourplanner.ValidationRules     5   	   0localclr-namespace:SWE2_Tourplanner     5   	   $	��400��5	   *   $	��550��67     � Title  6	   �� *HeadingMessage+.��5
      �  � ColumnDefinitions 5   
   �� 5      $��1* �6   �� 5      $��1* �6   .+�5   
   � $��0��6   $��5q�6    � RowDefinitions 5      �� 5      $��1* �6    �� 5      $��1* �6    �� 5      $��1* �6    �� 5      $��1* �6    .+�5      � $��0��6    5      �� 5      $��1* �6'   �� 5      $��1* �6'   .+�5      ��  �� Orientation$ Vertical=�6   $��0��64   $��2q�6D   .+�5      �� $��Name: ��6   �� 5         errorTemplate =� ErrorTemplate# [  6   .��6X   ��5      �� 5       �� Path$
 Name�6&    �� ValidatesOnDataErrors$
 True��62    �� UpdateSourceTrigger$ PropertyChanged=�6O    �� ValidationRules 5    "   >   8SWE2_Tourplanner.ValidationRules.TextInputValidationRule  5!   &   
	   Min$	 1��6I   

   Max$
 75��6Q   5#      .��5$   (   5%      5&      �� 5'      $ Vertical=�6   $��1��64   $��2q�6D   .+�5(      �� $��Start location: ��6   �� 5)      # [  6   .��6X   ��5*      �� 5+      $ StartLocation�6&   $
 True��6;   $ PropertyChanged=�6X    5,   "     5-   &   $	 1��6I   $	
 150��6Q   5/      .��50   (   51      52      53      � 54      $��1��6    55      �� 56      $��1* �6'   �� 57      $��1* �6'   .+�59      �� $ Vertical=�6   $��0��64   $��2q�6D   .+�5:      �� $��End location: ��6   �� 5;      # [  6   .��6X   ��5<      �� 5=      $ EndLocation�6&   $
 True��69   $ PropertyChanged=�6V    5>   "     5?   &   $	 1��6I   $	
 150��6Q   5A      .��5B   (   5C      5D      �� 5E      $ Vertical=�6   $��1��64   $��2q�6D   .+�5F      �� $��Route type: ��6   �� 5G      ��6   �� *
RouteTypes+ �� SelectedItem 6B   �� *	RouteType+5H      5J      � 5K      $��2��6    5L      �� 5M      $
��1.5* �6'   �� 5N      $
��2.5* �6'   .+�5P      �� $��Description: ��6   $��0��60   �� 5Q       �� TextWrapping$
 Wrap=�67    �� AcceptsReturn$
 True��6K   " �� VerticalScrollBarVisibility$ Visible=�6`   $��1��6�   ��6   �� *Description+6�   5R      �� 5S      $��3��6   $��0 5 0 0q�6"   $
��Exit��63   ��6B   �� *ExitCommand+5T      � 5U   
   $��1��6   $��5q�6    5V      �� 5W      $
��0.5* �6    �� 5X      $
��1.5* �6    �� 5Y      $��1* �6    �� 5Z      $��1* �6    .+�5\      �� $��0��6   $!��Create/Update Tour manually��6$    d� 	IsChecked 6J   �� *
ManualTour+� 5]      $��1��6    5^      �� 5_      $��1* �6'   �� 5`      $��1* �6'   .+�5b      �� $ Vertical=�6   $��0��64   $��2q�6D   .+�5c      �� $��
Distance: ��6   �� 5d      # [  6?   {�6   �� *
ManualTour+.��6y   ��5e      �� 5f      $ Distance�6&   $
 True��66   $ PropertyChanged=�6S    �� Delay$	 250��6y    5g   "   >   8SWE2_Tourplanner.ValidationRules.NoNegativeDistancesRule  5h   &   5j      .��5k   (   5l      5m      �� 5n      $ Vertical=�6   $��1��64   $��2q�6D   .+�5o      �� $��Select image:��6   66   �� 5p      $��Open dialog��6   ��63   �� *SelectRouteInfoCommand+{�6^   �� *
ManualTour+�� 5q      $
��Wrap=�6;   ��6    �� *	RouteInfo+6;   5r      5s      �� 5t      $��2��6   .+�5u      �� $��Manage Maneuvers��6   ��64   �� *ManageManeuverCommand+5v      �� 5w      $��3��6   $��0 5 0 0q�6"   ��63   �� *ConfirmButtonMessage+��6\   �� *CommitCommand+5x      5y      5z      