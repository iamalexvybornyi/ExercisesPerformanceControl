   M S B A M L   `   `   `  ����  ExercisesPerformanceControl1    +ExercisesPerformanceControl.ExerciseControl   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$Y  SКонтроль правильности выполнения упражнения��5   	   $	��735��6=   $
��1024��6J    � 
ResizeMode$ NoResize=�6�   .��5      � -   5        $��
layoutGrid��5      $��	10 0 10 0q�6    � RowDefinitions 5   
   �� 5	      $
��Auto �6   �� 5
      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5   
   �� -   5        $
��Reps��5       J� Opacity$	 0.5Q�6    
���   �$��0,0,0,0q�6A   $��Right=�6R   $��Bottom=�6n   
�������$��Segoe UI)�6�   $��58(�6�   $��78��6�   $	��804��6�   06�   (� 5   
   $��1��6    (� Stretch$ Uniform=�6   $��Right=�61   .��5      � .+�5      �� -   5        $+��%ImageForLiveDataWithRemovedBackground��5      $	��420��6E   $	��480��6Q   �� 5      -   5        $��ImageForLiveData��5      $	��420��60   $	��480��6<   5      5      (� 5   
   $��1��6   $ Uniform=�6   $
��Left=�61   .��5      � $	��399��6   .+�5      �� -   5        $/��)ImageForRecordedDataWithRemovedBackground��5      $	��480��6I   $	��420��6V   �� 5      -   5        $��ImageForRecordedData��5      $	��480��64   $	��420��6A   r� 5      -   5        $��VideoControl��5       � Triggers 5      :� 5       :� RoutedEvent$ MediaElement.Loaded��6'   [�5      �� 5   "   .��5   &   �� .��5"   *   p� -	   5         g�Name$ VideoControlTimeline��5"   8    �� 
TargetName$ VideoControl��6T   	 g� RepeatBehavior$	 Forever��5#      5%   '   5&   #   5(      5*      5+      5,      5-      5.      