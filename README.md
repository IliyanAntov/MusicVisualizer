# Визуализатор на музика<br/>Music visualizer

# [Официална документация](./Documents/MusicVisualizer_Documentation_Iliyan_Antov.pdf)
# [Презентация на проекта](https://docs.google.com/presentation/d/1Hubuf1_2Yw28AFvXF5u0pnuvvKj3GJBz7yWSgTEtRgM/edit?usp=sharing)

## Демонстрация (Demo):
https://user-images.githubusercontent.com/26661552/114189560-ff07c780-9952-11eb-97ac-d56dab42e3f6.mp4

*Визуализатор на музика, базиран на WS2812B лента LED пиксели. С помощта на собствен алгоритъм за засичане на ритъм (Beat detection algorithm), базиран на анализ на Фурие (FFT), приложението изпраща информация към микроконтролер Arduino Uno R3. Предаването на информация става посредсвром серийна връзка през USB кабел и собствен протокол. Микроконтролерът обработва информацията и визуализира ритъма върху свързаната към него лента LED пиксели.*

## Блок схема:
![](./Documents/BlockScheme.png)

## Електрическа схема:
![](./Documents/Schematic.png)

## Използвани технологии:

* [C/C++ (Arduino)](https://www.arduino.cc/reference/en)
* [Arduino Uno R3](https://www.arduino.cc/reference/en)
* [WS2812B](https://cdn-shop.adafruit.com/datasheets/WS2812B.pdf)

## Автор:

Илиян Антов - [Iliyan Antov](https://github.com/IliyanAntov) - [i.antov2@gmail.com](i.antov2@gmail.com)
