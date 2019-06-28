#include <FastLED.h>
#define NUM_LEDS 300
#define DATA_PIN 6
#define LED_TYPE WS2811
#define COLOR_ORDER GRB

#define BRIGHTNESS 30
#define FRAMES_PER_SECOND 100

int ledsToLight;
char input;
int num;
int shiftAmount = 2;

CRGB leds[NUM_LEDS];

void setup() {
  delay(2000);
  FastLED.addLeds<LED_TYPE,DATA_PIN,COLOR_ORDER>(leds, NUM_LEDS).setCorrection(TypicalLEDStrip);
  FastLED.setMaxPowerInVoltsAndMilliamps(5, 8000); 
    FastLED.setBrightness(BRIGHTNESS);

  Serial.begin(115200);
  pinMode(LED_BUILTIN, OUTPUT);

  for(int i = 0; i < NUM_LEDS ; i++) {
      leds[i] = CRGB(0,0,0);
  }
  input = "e";
}



void loop() {
  if(Serial.available() > 0){
    input = Serial.read();
    Serial.println(input);
  }
  
  if(input == 's'){
    digitalWrite(LED_BUILTIN, HIGH);
    do{
      input = '0';
      if(Serial.available() > 0){
        input = Serial.read();
      }
      
      if(input == 'e' || (input - '0' > 9 || input - '0' < 0)){
          break;  
      }
      num = input - '0';
      ledsToLight = num;
      ShowLeds(ledsToLight);
      
    }while(input != 'e');
  }
  
  else if(input == 'e'){
    digitalWrite(LED_BUILTIN, LOW);
    for(int i = 0; i < NUM_LEDS ; i++) {
      leds[i] = CRGB(0,0,0);
    }
    FastLED.show(); 
  }

  else if (input == 'l'){ //slow
    shiftAmount = 1;
  }
  
  else if (input == 'm'){ //slow
    shiftAmount = 2;
  }
  else if (input == 'f'){ //slow
    shiftAmount = 3;
  }
  else if (input == 'v'){ //slow
    shiftAmount = 4;
  }
}

void ShowLeds(int ledsToLight){
  leds[0].red = 0;
  
  for(int i = NUM_LEDS-1; i > shiftAmount; i--){
    leds[i] = leds[i-shiftAmount];
  }
  for(int i = 1; i < shiftAmount + 1; i++){
    if(ledsToLight >= i){
      leds[i].red = 100;
    }
    else{
      leds[i].red = 0;
    }
  }
  FastLED.show();  
  FastLED.delay(1000/FRAMES_PER_SECOND); 
}
