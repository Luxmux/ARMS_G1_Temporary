   M S B A M L   `   `   `  ����K  GARMS_WPF_G1_Temp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:ARMS_WPF_G1_TempARMS_WPF_G1_Temp  5   	       ARMS_WPF_G1_Temp.About   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0localclr-namespace:ARMS_WPF_G1_Temp     5   	     � Title$  ARMS WPF Temp - About��5   	   $	��448��6'   $	��913��64   ���#f0f0f0 � Icon$ icon.jpg��6U    � 
ResizeMode$ CanMinimize=�6e    � 	Resources 5	       �� 
TargetType �� Property �� Value �� 
TargetType �� Triggers	 P� Property
 P� Value ��Name �� Triggers �� 
TargetName��%�  '��       '�� D    �� 5   
   " ��6   .�5      ��  � ��$ 1q�60   �� 5       � �� �#0000006,   �� 5       � �� �#e6e7e46+   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� $��1q�6!   
���   �#��z��6J   ._�5      �� $��Center=�6/   $��Center=�6L   5      5      5       5      P� 5      	 �@J�IsMouseOver
 . . �5      ��  � �� �#d0d0d063   5      P� 5      	 � {�
 .  . �5      ��  � �� �#39496963   �� 5       � �� �#b9b9b963   5      5"      �� 5$   
   " ��6   .�5%      ��  �@�OverridesDefaultStyle . 66   �� 5&       �@�FocusVisualStyle 61   Z� �� 5'       � �� 5(      �� 5)      " ��6&   .R�5*      �� $��Border��6"   
���   �$��1q�6D   ._�5+      �� $ PART_ContentHost��66   $��0q�6+   66   5,       5-      P� 5.      	 � {�
 .  . �5/   "   �� $ Border��6)    � �� �#e6e7e46S   �� 50   "   $ Border��6)    � ��
 �   �6T   51      P� 52      	 � {�
 . . �53   "   �� $ Border��6)    � ��
 �����6S   �� 54   "   $ Border��6)    � ��
 �   �6T   55      57      59      5:      5A      .��5B      � $��
-3,0,-3,-3q�6    � ColumnDefinitions 5C   
   �� 5D      $
��109* �6   �� 5E      $
��804* �6   .+�5G   
   �� -   5         ��Name$
 logo��5G      $
��Left=�6   $	��245��69   $	��262��6F   $��
27,-12,0,0q�6R   $	��Top=�6f   $��	logo2.PNG��6~   $��2��6�   �� 5H   
   $��Calibri)�6   $��13(�6'   $
��Left=�65   $	��302��6P   $��
22,273,0,0q�6\   $	��Top=�6p   $��32��6�   $��2��6�   .��5I      �� $5�/Copyright © 2021 Luxmux Technology Corporation��6   5J      �� 5K   
   -   5         ��Name$ LightSourceLabel��5K   B   $-��'ARMS WPF Temp Integrated Spectral Bench��6   $��Calibri)�6\   $��14(�6q   $
��Left=�6�   $��
22,196,0,0q�6�   $	��Top=�6�   $��27��6�   $	��302��6�   $��2��6�   �� 5L   
   -   5        $ ModelumberLabel��5L   ,   $��Model Number: N/A��6   $��Calibri)�6E   $��13(�6Z   $
��Left=�6i   $��
22,224,0,0q�6�   $	��Top=�6�   $��27��6�   $	��302��6�   $��2��6�   �� 5M   
   -   5        $ SoftwareVersionLabel��5M   ,   $��Software Version:��6   $��Calibri)�6J   $��13(�6_   $
��Left=�6n   $��
22,249,0,0q�6�   $	��Top=�6�   $��25��6�   $	��302��6�   $��2��6�   �� 5N   
   $��1q�6   
�������$��221,64,18,170q�68   $��1��6O   �� 5O   
   $��Capabilities��6   $
��Left=�6'   $��
231,50,0,0q�6B   $	��Top=�6V   ���#f0f0f0$��1��6�   �� 5P   
   $��BlackBody Lamp��6   $
��Left=�6)   $��
231,78,0,0q�6D   $	��Top=�6X   $��1��6p   �� 5Q   
   -   5        $ SLED1��5Q      $	��N/A��6   $
��Left=�6-   $��
338,79,0,0q�6H   $	��Top=�6\   $��24��6u   $	��200��6�   $��1��6�   �� 5R   
   $��Power Meter��6   $
��Left=�6&   $	��Top=�6A   $��231,158,0,0q�6Z   $��1��6o   �� 5S   
   $��Firmware Version��6   $
��Left=�6+   $	��Top=�6F   $��231,210,0,0q�6_   $��1��6t   �� 5T   
   -   5        $ Power_Meter��5T      $	��N/A��6   $
��Left=�63   $��338,159,0,0q�6N   $	��Top=�6c   $��24��6|   $	��200��6�   $��1��6�   �� 5U   
   -   5        $ Firmware_Version��5U      $	��N/A��6   $
��Left=�68   $��338,209,0,0q�6S   $	��Top=�6h   $��28��6�   $	��200��6�   $��1��6�   �� 5V   
   -   5         ��Name$ 	Close_But��5V      $��Calibri)�6$   $��11(�69   $��Close��6^   {�. $
��Left=�6   $��23��6�   $��82��6�   $��697,304,0,0q�6�   $	��Top=�6�   $��1��6�   �� 5W   
   $��Power Meter��6   $
��Left=�6&   $	��Top=�6A   $��231,158,0,0q�6Z   $��1��6o   �� 5X   
   -	   5        $ Power_Meter_Copy��5X      $	��N/A��6   $
��Left=�68   $��338,159,0,0q�6S   $	��Top=�6h   $��24��6�   $	��200��6�   $��1��6�   �� 5Y   
   $��Power Meter��6   $
��Left=�6&   $	��Top=�6A   $��232,134,0,0q�6Z   $��1��6o   �� 5Z   
   -
   5        $ Power_Meter_Copy1��5Z      $	��N/A��6   $
��Left=�69   $��339,135,0,0q�6T   $	��Top=�6i   $��24��6�   $	��200��6�   $��1��6�   �� 5[   
   $��Gas 1��6   $
��Left=�6    $	��Top=�6;   $��231,108,0,0q�6T   $��1��6i   �� 5\   
   -   5        $ Power_Meter_Copy2��5\      $	��N/A��6   $
��Left=�69   $��338,109,0,0q�6T   $	��Top=�6i   $��24��6�   $	��200��6�   $��1��6�   �� 5]   
   $��Power Meter��6   $
��Left=�6&   $	��Top=�6A   $��231,187,0,0q�6Z   $��1��6o   �� 5^   
   -   5        $ Power_Meter_Copy3��5^      $	��N/A��6   $
��Left=�69   $��338,188,0,0q�6T   $	��Top=�6i   $��24��6�   $	��200��6�   $��1��6�   5_      5b      