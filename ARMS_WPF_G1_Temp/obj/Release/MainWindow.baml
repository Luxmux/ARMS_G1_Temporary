   M S B A M L   `   `   `  ���� K  GARMS_WPF_G1_Temp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:ARMS_WPF_G1_TempARMS_WPF_G1_Temp  O KSliderWithTickLabels, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullY@clr-namespace:SliderWithTickLabels;assembly=SliderWithTickLabelsSliderWithTickLabels U QLiveCharts.Wpf, Version=0.9.7.0, Culture=neutral, PublicKeyToken=0bc1f845d1ebb8dfG4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.WpfLiveCharts.Wpf !    ARMS_WPF_G1_Temp.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     0localclr-namespace:ARMS_WPF_G1_Temp     Scustom@clr-namespace:SliderWithTickLabels;assembly=SliderWithTickLabels     DWpf4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf       � 
ResizeMode$  CanMinimize=� � Title$ ARMS WPF Temp��$	��714��$��1041.714�� � Icon$ icon.jpg�� � 	Resources  �� 
TargetType �� Property �� Value �� 
TargetType �� Triggers	 P� Property
 P� Value   RegularTextBox ��Name �� Triggers �� 
TargetName  HeatorCoolTextBox ��Name J� SnapsToDevicePixels  dayTimeSlider m� 	Placement m� Fill ]� DecreaseRepeatButton ]� IncreaseRepeatButton ]� Thumb �� CornerRadius J� Opacity �� RelativeSource �� AncestorType �� StringFormat � LayoutTransform �� Angle��%�  '��       &	  K    &	 �    &	 }    �� " ��.���  � ��$ 1q���  � �� �#000000��  � �� �#e6e7e4��  � �� �� " ��.R��� $��1q�
���   �#��z��._��� $��Center=�$��Center=� P� 	 �@J�IsMouseOver
 . . ���  � �� �#d0d0d0P� 	 � {�
 .  . ���  � �� �#394969��  � �� �#b9b9b9�� " ��.���  � �� �� " ��.R��� $��Border��
���   �$��1q�._��� $ PART_ContentHost��$��0q� P� 	 � {�
 .  . ��� $ Border�� � �� �#e6e7e4�� $ Border�� � ��
 ���p���  � �� �#494949P� 	 � {�
 . . ��� $ Border�� � ��
 ������� $ Border�� � ��
 �   ���  � ��
 �   �P� 	 �@J�IsMouseOver
 . �� " ��.���  � �� �� " ��.R��� $ border��#��z��#��z��#��z�� . ._��� $ PART_ContentHost��|�.  $��Hidden=�$��Hidden=� �� " ��.���  �@�OverridesDefaultStyle . ��  � �� �� .R�� .+�m� $��LeftTick��$��0��$��10��$
 Left=�$y�	Collapsed=�# z��m� $��	RightTick��$��10��$ Right=�$y�	Collapsed=�# z��]� $��
PART_Track�� �� $��Slider.DecreaseLarge������ " ��.���  �@�OverridesDefaultStyle . ��  � �� �#2259FF��  � �� �#2259FF��  �@J�Opacity$	 0.7Q���  � ��$ Slider.DecreaseLarge����  � |� .  ��  � �� �� " ��.R��� #��z��#��z��#��z�� P� 	 �@J�IsMouseOver
 . . ���  � �� �#2259FFP� 	 � {�
 .  . ���  � �� �#b9b9b9 �� $��Slider.IncreaseLarge������ " ��.���  �@�OverridesDefaultStyle . ��  � �� �#F5F5F5��  � �� �#F5F5F5��  �@J�Opacity$ 0Q���  � |� .  ��  � �� �� " ��.R��� #��z��#��z��#��z�� P� 	 �@J�IsMouseOver
 . . ���  � �� �#F5F5F5 n� ���� " n�.���  � �� �� " n�.R���  . .+�� $��42��$��15��.+��� $ 
Background�����#a2a2a2$��1q�$��0��$��2��$ 5,5,5,5��$��42��$��15��
���   �$ 90Q��� $��Center=�$��Center=�
���   �$��9(����� *Value+ �� " ��$ {0:N1}�� �� $ 90Q�.���� ���#f5f5f5$��15q�
�������$	��751��$	��Top=�$��	0,0,-8,-5q�.��� -    �Name$ main_window_grid�����#f5f5f5$��0,0,0,0q�$
��1210��$	��747�� � RowDefinitions �� $	��65* ��� $
��309* � � ColumnDefinitions �� $
��285* ��� $
��130* ��� $	��12* ��� $
��783* �.+��� $	��414��$	��Top=�$	��484��$
��Left=�$ 8�����#f5f5f5$��1q�
�������$��	10,-1,0,0q�$��3��$��2���� -    ��Name$ datedisplay��$��Calibri)�$��11(����#f5f5f5$��0q�
�������$��Right=�$	��412��$	��Top=�$��171,519,0,0q�$��36��$
��Left=�$	��Top=�$
��date��$��3��$��1���� -   $ displaybottom��$��Calibri)�$��11(�$�� �����#f5f5f5$��0q�
�������$
��Left=�$	��700��$	��Top=�$��	6,521,0,0q�$��34��$
��Left=�$	��Top=�$��4��$��1���� $	��615��$
��Left=�$ 8�����#f5f5f5$��1q�
�������$��123.857,-1,0,366.857q�$��1��$��2��$��3���� $	��415��$	��Top=�$��
10,359,0,0q�$	��104��$
��Left=�$ 8�����#f5f5f5$��1q�
�������$��3��$��1���� $��Calibri)�$��13(�$��Center=�$��Center=�$	��134��$	��Top=�$��	7,355,0,0q�$��24��$
��Left=�$��System Temperatures��$��1���� -     ��Name$  Diagnostics��#��[  $��Calibri)�$��11(�$y�Visible=�$
��Auto=�$��Visible=�$
��Left=�$��3.826,256.286,0,0q�$	��207��$	��Top=�$	��594��! J� RenderTransformOrigin$! 0.398,0.307&�$��3��$��1���� -   " ��Name$" AdminBut��$��Calibri)�$��11(�{�. $
��Left=�$	��234��$��14,31,0,527q�$��0q����#f5f5f5$! 0.778,0.568&�$��2������ " ��.���  � �� �� " ��.R��� #��z��._���  P� 	 �@J�IsMouseOver
 . . ���  � �� �#f5f5f5.���� $��Stretch=����#f5f5f5# � Cursor$
# Hand��$��	0,-29,2,1q�.+��� $
��Left=�$��	logo2.png��$	��240��$	��298��$! 0.453,0.309&�$��	0,0,-66,0q�z�V� .~��� �� $ �� AngleX$$ -0.06Q��� T� % T� X$% 0.172Q��� $��Calibri)�$��13(�$��Center=�$��Center=�$��87��$��Bottom=�$��
9,-6,0,111q�$��25��$
��Left=�$��Lamp Control��$! 0.556,-0.125&��� $��Calibri)�$��11(�$��Center=�$��Center=�$��	Lamp (mA)��$
��Left=�$	��Top=�$��24��$��4,33,0,0q�$��80��$! 0.447,-0.036&�$��1���� -   $  setCurr1Edit��$�� ��#��[  $��Calibri)�$��11(�$��Center=�$��Center=�$��54��$	��Top=�$��
17,281,0,0q�$��22��$
��Left=�
���   �$��1q�$! 	0.757,0.5&�$��1��$��1���� -   $  actualCurr1Edit��#��[  $��Calibri)�$��11(�$��Center=�$��Center=�$��54��$	��Top=�$��
17,313,0,0q�{�.  $��22��$
��Left=�
���   �$��1q�$! 	0.757,0.5&�$��1��$��1���� $��Calibri)�$��11(�$��Center=�$��Center=�$��94��$	��Top=�$��	81,64,0,0q�$��26��$
��Left=�$��
Lamp Power��$��1���� $��Calibri)�$��11(�$��Center=�$��Center=�$��94��$	��Top=�$��
81,133,0,0q�$��24��$
��Left=�$��Lamp Current��$��1���� -   $" maxBut��$��Calibri)�$��11(�$��Set Max��$��Center=�$
��Left=�$��
92,155,0,0q�$	��Top=�$��72��$��24��$��1���� -	   $" minBut��$��Calibri)�$��11(�$��Set Min��$��Center=�$
��Left=�$��
92,185,0,0q�$	��Top=�$��72��$��22��$��1���� $��Calibri)�$��11(�$��Right=�$��Center=�$��94��$	��Top=�$��194,281,0,0q�$��24��$
��Left=�$��Set Current��$��1��$��2���� $��Calibri)�$��11(�$��Right=�$��Center=�$��94��$	��Top=�$��193,313,0,0q�$��24��$
��Left=�$��Actual Current��$��1��$��2���� $��Calibri)�$��11(�$��Right=�$��Center=�$��91��$	��Top=�$��
11,407,0,0q�$��24��$
��Left=�$��Thermopile Temp��$��1���� -
   $  thermopileTempEdit��#��[  $��Calibri)�$��11(�$��Center=�$��Center=�$��54��$	��Top=�$��102,409,0,0q�{�.  $��20��$
��Left=�
���   �$��1q�$! 	0.757,0.5&�$��1���� -   $  fanSpeedReadEdit��#��[  $��Calibri)�$��11(�$��Center=�$��Center=�$��54��$	��Top=�$��236,407,0,0q�{�.  $��22��$
��Left=�
���   �$��1q�$! 	0.757,0.5&�$��2��$��1���� -   $  fanSpeedSetPointEdit��#��[  $��Calibri)�$��11(�$��Center=�$��Center=�$��54��$	��Top=�$��
15,407,0,0q�{�. $��22��$
��Left=�
���   �$��1q�$! 	0.757,0.5&�$��1��$��1���� -   $" FanSpeedSetBut��$��Calibri)�$��11(�$	��Set��$
��Left=�$��
76,407,0,0q�$	��Top=�$��54��$��22��$��1��$��1���� $��Calibri)�$��11(�$��Center=�$��Center=�$��72��$	��Top=�$��227,389,0,0q�$��22��$
��Left=�$
��Read��$��2��$��1���� $��Calibri)�$��11(�$��Center=�$��Center=�$��54��$	��Top=�$��
15,389,0,0q�$��22��$
��Left=�$	��Set��$��1��$��1���� $��Calibri)�$��11(�$��Center=�$��Center=�$	��110��$	��Top=�$��272,427,0,0q�$��22��$
��Left=�$��(0.0-7.0 CFM)��$��1��$��2���� $��Calibri)�$��13(�$��Center=�$��Center=�$��79��$	��Top=�$��3,-7,0,0q�$��27��$
��Left=�$��Gas Sensors��$! 0.556,-0.125&�$��3���� -   $" Start_PM��$��Calibri)�$��11(�$��Start��$
��Left=�$��	30,28,0,0q�$	��Top=�$��54��$��21��$��3���� -   $" Stop_PM��$��Calibri)�$��11(�$
��Stop��$
��Left=�$��	91,28,0,0q�$	��Top=�$��54��$��21��$��3���� -   $" Clear_PM��$��Calibri)�$��11(�$��Clear��$
��Left=�$��
154,28,0,0q�$	��Top=�$��54��$��21��$��3���� -   & ��Name$& List_PM��$��Calibri)�$��11(�$
��Left=�$��Center=�$��
440,28,0,0q�$	��Top=�$	��116��$��21��$! 1.389,1.117&�$��3��.@��� -   ' ��Name$' Gas1_Raw��$��Gas 1 [Raw]��
��������� -   $' Gas1_V��$��	Gas 1 [V]��
��������� -   $' Gas2_Raw��$��Gas 2 [Raw]��
��������� -   $' Gas2_V��$��	Gas 2 [V]��
��������� -   $' Gas3_Raw��$��Gas 3 [Raw]��
��������� -   $' Gas3_V��$��	Gas 3 [V]��
��������� -   $' Gas4_Raw��$��Gas 4 [Raw]��
��������� -   $' Gas4_V��$��	Gas 4 [V]��
��������� -   $" 
sledsOnBut��$��Calibri)�$��11(�$��         Off��$��Center=�$
��Left=�$��36��$��
101,90,0,0q�$	��Top=�$��54��$! 1.161,-0.158&�$��1��A� -   ( A�Name$( sledsOnIndicator�����#CD5C5C
���   �$
��Left=�$��14��$��111,102,0,0q�$	��Top=�$��14��$��1��#  LiveCharts.Wpf.CartesianChart  -   )  Name$) ValuesChart��$y�Visible=�$
��Left=�&   LiveCharts.Wpf.Charts.Base.Chart*   LegendLocation$
* None=�$	��Top=�$	��240��$��5,57,0,0q�$	��560��+   DisableAnimations+ . ,   	Hoverable, .  $��3��$��2��-   AxisY-   LiveCharts.Wpf.Axis  -   .  Name$. ValuesChart_YAxis��/   Title$/ Power��0   IsMerged0 .  1   MinValue$1 
1000000000Q�2   MaxValue$2 -1000000000Q�3   	Separator3   LiveCharts.Wpf.Separator  -   4  Name$4 ValuesChart_YAxis_Ticks���� $��Calibri)�$��11(�$��Center=�$��Center=�$��76��$	��Top=�$��163,407,0,0q�$��22��$
��Left=�$��	Fan Speed��$��1���� $��42��$	��Top=�$	��343��$
��Left=�$��1q�
���   �$��	23,59,0,0q�$��2��$��1��/  )SliderWithTickLabels.SliderWithTickLabels  -   5  Name$5 slider1TrackBar��#��[ 
���   �$��1q�$��Center=�$��Center=�6 �� IsDirectionReversed6 .  7 �� Orientation$7 
Horizontal=�$	��328��8 �� TickPlacement$8 TopLeft=�9 � Minimum$9 0Q�: � Maximum$	: 550Q�; �� TickFrequency$	; 100Q�< �� IsSnapToTickEnabled< . $! 0.5,0.5&�$��165,87,717,488q�$��42��$��4��$��1��z�V� .~��� $	 -90Q�=   TickLabelTemplate= �� .R�� $��30��$��21����!� > !� Visual> A� $��20��$��21��
�������.+��� $��Calibri)�$��9(�$��Center=�$��Center=�$��Center=�$��Center=�$��0q�
���   �$! 0.5,0.5&����� z�V� .~��� �� �� $	 450Q�T� �� -    $" 
LoggingBut��$��Calibri)�$��11(�$��      Logging��{�. $
��Left=�$��88��$��97,474,0,104q�$��3��$��1��A� -!   $( LoggingIndicator�����#CD5C5C
���   �$
��Left=�$��14��$��114,488,0,0q�$	��Top=�$��13��$! 6.769,1.308&�$��3��$��1���� -"   $" AboutBut��$��Calibri)�$��11(�$��About��{�. $
��Left=�$��88��$��278,474,0,104q�$! 
0.825,0.56&�$��3��$��1���� -#   $" DefaultsBut��$��Calibri)�$��11(�$��Set Defaults��{�. $
��Left=�$��88��$��188,474,0,104q�$! 0.778,0.568&�$��3��$��1���� -$   $" CommunicationBut��$��Calibri)�$��11(�$��Communications��{�. $
��Left=�$��88��$��5,474,0,104q�$! 0.778,0.568&�$��3��$��1���� -%   $" ExportBut_PM��$��Calibri)�$��11(�$��Export��$
��Left=�$��487.826,177.286,0,0q�$	��Top=�$��75��$��20��$��3��$��1���� 	? �� X1$? 10��	@ �� Y1$@ 15��	A �� X2$	A 424��	B �� Y2$B 15��
�������$	��0.6��$��4��$��2���� $	? 430��$@ 15��$
A 1012��$B 15��
�������$	��0.6��$��4��$��2���� $	? 430��$	@ 390��$
A 1012��$	B 390��
�������$	��0.6��$��4��$��2��$��
0,-57,0,57q��� $? 10��$	@ 625��$	A 424��$	B 625��
�������$	��0.6��$��4��$��2��$��0,-119,0,119q��� $��Calibri)�$��11(�$��Center=�$��Center=�$��94��$	��Top=�$��
81,220,0,0q�$��26��$
��Left=�$��Lamp Operation��$��1���� -&   $" lampEnableBut��$��Calibri)�$��11(�$��         Unlocked��$��Center=�$
��Left=�$��36��$��
85,246,0,0q�$	��Top=�$��86��$! 1.161,-0.158&�$��1��A� -'   $( lampEnableIndicator�����#CD5C5C
���   �$
��Left=�$��14��$��
98,257,0,0q�$	��Top=�$��14��$��1���� -(   $  Gas1Edit��#��[  $��Calibri)�$��11(�$��Center=�$��Center=�$��80��$	��Top=�$��64.826,215.286,0,0q�{�.  $��22��$
��Left=�
���   �$��1q�$! 	0.757,0.5&�$��1��$��3���� $��Calibri)�$��11(�$��Center=�$��Center=�$��55��$	��Top=�$��12.826,215.286,0,0q�$��22��$
��Left=�$��Gas 1:��$��1��$��3���� -)   $  Gas2Edit��#��[  $��Calibri)�$��11(�$��Center=�$��Center=�$��80��$	��Top=�$��202.826,217.286,0,0q�{�.  $��22��$
��Left=�
���   �$��1q�$! 	0.757,0.5&�$��1��$��3���� $��Calibri)�$��11(�$��Center=�$��Center=�$��55��$	��Top=�$��150.826,215.286,0,0q�$��22��$
��Left=�$��Gas 2:��$��1��$��3���� -*   $  Gas3Edit��#��[  $��Calibri)�$��11(�$��Center=�$��Center=�$��80��$	��Top=�$��346.826,218.286,0,0q�{�.  $��22��$
��Left=�
���   �$��1q�$! 	0.757,0.5&�$��1��$��3���� $��Calibri)�$��11(�$��Center=�$��Center=�$��55��$	��Top=�$��294.826,217.286,0,0q�$��22��$
��Left=�$��Gas 3:��$��1��$��3���� -+   $  Gas4Edit��#��[  $��Calibri)�$��11(�$��Center=�$��Center=�$��80��$	��Top=�$��484.826,218.286,0,0q�{�.  $��22��$
��Left=�
���   �$��1q�$! 	0.757,0.5&�$��1��$��3���� $��Calibri)�$��11(�$��Center=�$��Center=�$��55��$	��Top=�$��434.826,217.286,0,0q�$��22��$
��Left=�$��Gas 4:��$��1��$��3��