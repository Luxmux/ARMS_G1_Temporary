   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  O KSliderWithTickLabels, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:SliderWithTickLabelsSliderWithTickLabels \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   Zero  TickLabel.Foreground  �� Color ! SliderThumb.Static.Foreground $  SliderThumb.MouseOver.Background   SliderThumb.MouseOver.Border " SliderThumb.Pressed.Background  SliderThumb.Pressed.Border # SliderThumb.Disabled.Background  SliderThumb.Disabled.Border !	 SliderThumb.Static.Background 
 SliderThumb.Static.Border  SliderThumb.Track.Border   SliderThumb.Track.Background  TickLabelTemplate ��Name �� 
TargetType �� Property �� Value �� Triggers ^� 
Conditions �� Binding �� Path	 �� RelativeSource
 �� AncestorType �� Value  TickLabelContainer �Name �� Binding �� Value `� 	Converter6  0SliderWithTickLabels.TickBarLabelMarginConverter �� Source  RepeatButtonTransparent �� 
TargetType  SliderThumbHorizontalTop � UseLayoutRounding F�Name �� Stretch J� SnapsToDevicePixels �� Triggers P� Property P� Value �� 
TargetName  SliderThumbHorizontalBottom   SliderThumbHorizontalDefault  SliderHorizontal/  )SliderWithTickLabels.SliderWithTickLabels ��Name � RowDefinitions m�Name m� Fill m� 	Placement ��Name  ��Name! ]�Name" ]� DecreaseRepeatButton# ]� IncreaseRepeatButton$ ]� Thumb% n�Name& � OverridesDefaultStyle  SliderThumbVerticalLeft  SliderThumbVerticalRight  SliderThumbVerticalDefault  SliderVertical' � ColumnDefinitions( �� DecreaseLarge) �� IncreaseLarge�� D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     1system&clr-namespace:System;assembly=mscorlib  ,local"clr-namespace:SliderWithTickLabels  %�$  &	        &	      &	       &	 5     &	 H     &	 [     &	 n     &	 �     &	 �     &		 �     &	
 �     &	 �     &	 �     &	 �     7  &	 4    7   7   7   7   7   7   &	 W    &	 	    7 	 7 
 7  7  7  7  &	 1    7 	 7 
 7  7  7  7  &	 T    7 	 7 
 7  7  7  7  7  7  &	 E    7  7  7  7  7  7  7  &	 �    7 	 7 
 7  7  7  7  7  7  &	 �    7 	 7 
 7  7  7  7  7  7  &	 �    7 	 7 
 7  7  7  7  7  7  &	     7  7  7  7  7  7  7  7  7  7  7  '  �!    7  7  7  7  X�0Q��� $  	#FF000000���� $  	#FFE5E5E5���� $  #2259FF���� $  #2259FF���� $  #2259FF���� $  #2259FF���� $  #b9b9b9���� $  #b9b9b9���� $  #b9b9b9���� $  #b9b9b9���� $  #b9b9b9���� $  #b9b9b9���� .R��� $ 	TickLabel��8��  ���� ���� " ��.���  � ��$
 Left=���  � ��$	 Top=� ^�  ��  �� $ Orientation�	 �� *FindAncestor+$ 
 local:SliderWithTickLabelsL� Vertical��  �� $
 Name�	 �� *FindAncestor+$
 ContentPresenterL� TopTickLabel.0���  � ��$ Right=�^�  ��  �� $ Orientation�	 �� *FindAncestor+$ 
 local:SliderWithTickLabelsL� Vertical��  �� $
 Name�	 �� *FindAncestor+$
 ContentPresenterL� BottomTickLabel.0���  � ��$
 Left=��� .R��� ���� .R�� $ TickLabelContainer������  ��  �� $ Orientation�	 �� *FindAncestor+$ 
 local:SliderWithTickLabelsL� 
Horizontal.a���  � ��$ Center=���  � ��$	 Top=���  � �� `�     .1��� 	 �� *FindAncestor+$ 
 local:SliderWithTickLabelsL��� �� 8   �� 8  �� 8  ��  �� $ Orientation�	 �� *FindAncestor+$ 
 local:SliderWithTickLabelsL� Vertical.a���  � ��$ Center=���  � �� `�     .1��� 	 �� *FindAncestor+$ 
 local:SliderWithTickLabelsL��� 8  �� �� 8  �� 8  ^�  ��  �� $ Orientation�	 �� *FindAncestor+$ 
 local:SliderWithTickLabelsL� Vertical��  �� $
 Name�	 �� *FindAncestor+$
 ContentPresenterL� TopTickLabel.0���  � ��$ Right=�^�  ��  �� $ Orientation�	 �� *FindAncestor+$ 
 local:SliderWithTickLabelsL� Vertical��  �� $
 Name�	 �� *FindAncestor+$
 ContentPresenterL� BottomTickLabel.0���  � ��$
 Left=����� " ��.���  � �� �� $ GeneratedTicks�	 �� *FindAncestor+$ 
 local:SliderWithTickLabelsL���  � �� �� $ TickLabelTemplate�	 �� *FindAncestor+$ 
 local:SliderWithTickLabelsL��� " ��.���  �@�OverridesDefaultStyle . ��  � ��
 ���� ��  � |� .  ��  � �� .  ��  � �� �� " ��.R��� #��z��#��z��#��z���� " n�.R�� $��Center=� . $��Center=�.+�F� $
 grip�������    ��[ �    ��[ `�S `�S �`�S     �ا ��[ �ا ��[ ��ا ��[ �ا ���ا ����ا �������    ����[ ��[ 	8��  $
 Fill=� . 8�� $��1�� . $��Center=� P�  �@J�IsMouseOver . . ��� $
 grip��8   � ���� $
 grip��8   � ��P�  �@n�
IsDragging . . ��� $
 grip��8   � ���� $
 grip��8   � ��P�  � {� .  . ��� $
 grip�� � �� �#b9b9b9�� $
 grip�� � �� �#b9b9b9�� " n�.R�� $��Center=� . $��Center=�.+�F� $
 grip�������     � �     � `�S ��`�S ���`�S ���ا  � �ا  � ��ا  � �ا �ا ��ا     �         �  � 	8��  $
 Fill=� . 8�� $��1�� . $��Center=� P�  �@J�IsMouseOver . . ��� $
 grip��8   � ���� $
 grip��8   � ��P�  �@n�
IsDragging . . ��� $
 grip��8   � ���� $
 grip��8   � ��P�  � {� .  . ��� $
 grip�� � �� �#b9b9b9�� $
 grip�� � �� �#b9b9b9�� " n�.R�� $��Center=� . $��Center=�.+�F� $
 grip��d����        �        �ا �ا ��ا     �ا ���ا ����ا �������    ��	8��  $
 Fill=� . 8�� $��1�� . $��Center=� P�  �@J�IsMouseOver . . ��� $
 grip��8   � ���� $
 grip��8   � ��P�  �@n�
IsDragging . . ��� $
 grip��8   � ���� $
 grip��8   � ��P�  � {� .  . ��� $
 grip��8   � ���� $
 grip��8   � ���� "  .R��� $ border��#��z��#��z��#��z�� . ._��  �� $
��Auto ��� $
��Auto ��� $
��Auto �#��z���� $
��Auto ��� $
��Auto �.+�m� $ TopTick��# z��$��4��$��0,0,0,2q�$	 Top=�$��1��$y�	Collapsed=�m� $ 
BottomTick��# z��$��4��$��0,2,0,0q�$ Bottom=�$��3��$y�	Collapsed=��� $ TopTickLabel��$��0��$y�	Collapsed=�8��  �� $ BottomTickLabel��$��4��$y�	Collapsed=�8�� �� $ TrackBackground��8�� $��1q�8�� $	��4.0��$	��5,0q�$��2��$��center=�._��� $��-6,-1q�.+��� $  PART_SelectionRange��#��� ��$	��4.0��$y�Hidden=�]� $! 
PART_Track��$��2��" �� $��Slider.DecreaseLarge��
���  ��# �� $��Slider.IncreaseLarge��
�������$ n� $% Thumb��|�.  $��21��& . 8�� $��Center=�$��11�� P�  �@��TickPlacement$ TopLeft=�. ��� $ TopTick�� � y�$ Visible=��� $ TopTickLabel�� � y�$ Visible=��� $ Thumb��8   � ���� $ TrackBackground�� � ��$ 5,2,5,0q�P�  �@��TickPlacement$ BottomRight=�. ��� $ 
BottomTick�� � y�$ Visible=��� $ BottomTickLabel�� � y�$ Visible=��� $ Thumb��8   � ���� $ TrackBackground�� � ��$ 5,0,5,2q�P�  �@��TickPlacement$
 Both=�. ��� $ TopTick�� � y�$ Visible=��� $ TopTickLabel�� � y�$ Visible=��� $ 
BottomTick�� � y�$ Visible=��� $ BottomTickLabel�� � y�$ Visible=�P�  �@��IsSelectionRangeEnabled . . ��� $ PART_SelectionRange�� � y�$ Visible=�P�  �@J�IsKeyboardFocused . . ��� $ Thumb�� � ��
 ��  ��� " n�.R�� $��Center=� . $��Center=�.+�F� $
 grip���������[ �ا Ӏ�[ �ا `�S `�S �    `�S ��[ ��[ Ӏ�[     ����Ӏ�    ���ا ���ا Ӏ��ا ��[ �ا ��[ �ا 	8��  $
 Fill=�8��  P�  �@J�IsMouseOver . . ��� $
 grip��8   � ���� $
 grip��8   � ��P�  �@n�
IsDragging . . ��� $
 grip��8   � ���� $
 grip��8   � ��P�  � {� .  . ��� $
 grip��8   � ���� $
 grip��8   � ���� " n�.R�� $��Center=� . $��Center=�.+�F� $
 grip������� � �ا � � �ا ��`�S ��`�S Ӏ�`�S  �  � � �     �        �ا �ا �    �ا  � �ا  � �ا 	8��  $
 Fill=�8��  P�  �@J�IsMouseOver . . ��� $
 grip��8   � ���� $
 grip��8   � ��P�  �@n�
IsDragging . . ��� $
 grip��8   � ���� $
 grip��8   � ��P�  � {� .  . ��� $
 grip��8   � ���� $
 grip��8   � ���� " n�.R�� $��Center=� . $��Center=�.+�F� $
 grip��+���� �  � ѠI � ѠI�y� � � �y� 	8��  $
 Fill=�8��  P�  �@J�IsMouseOver . . ��� $
 grip��8   � ���� $
 grip��8   � ��P�  �@n�
IsDragging . . ��� $
 grip��8   � ���� $
 grip��8   � ��P�  � {� .  . ��� $
 grip��8   � ���� $
 grip��8   � ���� "  .R��� $ border��#��z��#��z��#��z�� . ._�� ' �� $
��Auto ��� $
��Auto ��� #��z��$
��Auto ��� $
��Auto ��� $
��Auto �.+�m� $ TopTick��$��1��# z��$��0,0,2,0q�$
 Left=�$y�	Collapsed=�$��4��m� $ 
BottomTick��$��3��# z��$��2,0,0,0q�$ Right=�$y�	Collapsed=�$��4���� $ TopTickLabel��$��0��$y�	Collapsed=�8��  �� $ BottomTickLabel��$��4��$y�	Collapsed=�8�� �� $ TrackBackground��8�� $��1q�8�� $��2��$��center=�$	��0,5q�$	��4.0��._��� $��-1,-6q�.+��� $  PART_SelectionRange��#��� ��$y�Hidden=�$	��4.0��]� $! 
PART_Track��$��2��" �� #��Z( 8�� # �� #��Z) 8�� $ n� $% Thumb��|�.  $��11��& . 8�� $	��Top=�$��18�� P�  �@��TickPlacement$ TopLeft=�. ��� $ TopTick�� � y�$ Visible=��� $ TopTickLabel�� � y�$ Visible=��� $ Thumb��8   � ���� $ TrackBackground�� � ��$ 2,5,0,5q�P�  �@��TickPlacement$ BottomRight=�. ��� $ 
BottomTick�� � y�$ Visible=��� $ BottomTickLabel�� � y�$ Visible=��� $ Thumb��8   � ���� $ TrackBackground�� � ��$ 0,5,2,5q�P�  �@��TickPlacement$
 Both=�. ��� $ TopTick�� � y�$ Visible=��� $ TopTickLabel�� � y�$ Visible=��� $ 
BottomTick�� � y�$ Visible=��� $ BottomTickLabel�� � y�$ Visible=�P�  �@��IsSelectionRangeEnabled . . ��� $ PART_SelectionRange�� � y�$ Visible=�P�  �@J�IsKeyboardFocused . . ��� $ Thumb�� � ��
 ��  �P�  �@J�IsMouseOver . . ��� 8 	  � ��P�  � {� .  . ��� 8 
  � ���� "  .���  �@��IsPressAndHoldEnabled .  ��  � ��
 ���� ��  � ��
 ���� ��  � ��8   ��  � ��8  ��  �@ TickLabelTemplate8   P�  �@��Orientation$ Vertical=�. ���  � ��8  