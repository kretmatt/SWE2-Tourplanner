   M S B A M L   `   `   `  ���� K  GSWE2-Tourplanner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullS.clr-namespace:SWE2_Tourplanner.ValidationRules SWE2_Tourplanner.ValidationRules  3clr-namespace:SWE2_TourplannerSWE2_Tourplanner  +    %SWE2_Tourplanner.CreateUpdateTourView   \ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     E
Validators.clr-namespace:SWE2_Tourplanner.ValidationRules     0localclr-namespace:SWE2_Tourplanner     $	��400��$	��550��  � Title  �� *HeadingMessage+.���  � ColumnDefinitions �� $��1* ��� $��1* �.+�� $��0��$��5q� � RowDefinitions �� $��1* ��� $��1* ��� $��1* ��� $��1* �.+�� $��0�� �� $��1* ��� $��1* �.+���  �� Orientation$ Vertical=�$��0��$��2q�.+��� $��Name: ����    errorTemplate =� ErrorTemplate# [  .������  �� Path$
 Name� �� ValidatesOnDataErrors$
 True�� �� UpdateSourceTrigger$ PropertyChanged=� �� ValidationRules >   8SWE2_Tourplanner.ValidationRules.TextInputValidationRule  
	   Min$	 1��

   Max$
 75��.���� $ Vertical=�$��1��$��2q�.+��� $��Start location: ���� # [  .������ $ StartLocation�$
 True��$ PropertyChanged=�   $	 1��$	
 150��.��� $��1�� �� $��1* ��� $��1* �.+��� $ Vertical=�$��0��$��2q�.+��� $��End location: ���� # [  .������ $ EndLocation�$
 True��$ PropertyChanged=�   $	 1��$	
 150��.���� $ Vertical=�$��1��$��2q�.+��� $��Route type: ���� ���� *
RouteTypes+ �� SelectedItem �� *	RouteType+� $��2�� �� $
��1.5* ��� $
��2.5* �.+��� $��Description: ��$��0����  �� TextWrapping$
 Wrap=� �� AcceptsReturn$
 True��" �� VerticalScrollBarVisibility$ Visible=�$��1������ *Description+�� $��3��$��0 5 0 0q�$
��Exit������ *ExitCommand+� $��1��$��5q� �� $
��0.5* ��� $
��1.5* ��� $��1* ��� $��1* �.+��� $��0��$!��Create/Update Tour manually�� d� 	IsChecked �� *
ManualTour+� $��1�� �� $��1* ��� $��1* �.+��� $ Vertical=�$��0��$��2q�.+��� $��
Distance: ���� # [  {��� *
ManualTour+.������ $ Distance�$
 True��$ PropertyChanged=� �� Delay$	 250�� >   8SWE2_Tourplanner.ValidationRules.NoNegativeDistancesRule  .���� $ Vertical=�$��1��$��2q�.+��� $��Select image:���� $��Open dialog������ *SelectRouteInfoCommand+{��� *
ManualTour+�� $
��Wrap=����� *	RouteInfo+�� $��2��.+��� $��Manage Maneuvers������ *ManageManeuverCommand+�� $��3��$��0 5 0 0q����� *ConfirmButtonMessage+���� *CommitCommand+