#include <FastLED.h>
#define NUM_LEDS 300
#define DATA_PIN 6
#define LED_TYPE WS2811
#define COLOR_ORDER GRB

#define BRIGHTNESS 5
#define MAX_BRIGHTNESS 125
#define FRAMES_PER_SECOND 100

char input; // s -> Start default visualization
            // a -> Start alternate visualization
            // e -> Turn off all LEDs / Stop visualization
            // l, m, f, v -> LED travel speed ([l]ow,[m]edium, [f]ast, [v]ery fast)

int beatStrength; // Recieved by the PC every time there is a beat.

int shiftAmount = 2; // How many spaces a single LED moves every cycle
int colorShiftSpeed = 1; // Determines how fast the color changes (factor)
float colorShiftMaxDelay = 50; // Maximum delay before the color changes (in ms)
float colorShiftDelay = (colorShiftMaxDelay / colorShiftSpeed); // Total delay for every color change (in ms)

CRGB colors = {0, 0, 255}; // FastLED color struct for calculations (Starting color => blue)
int colorCounter = 0; // Color change helper variable

float lastChange = millis(); // Used for the non-blocking delay in the ShiftColors() function

int brightnessFactor = 0; // (Alternate visualization) Determines how much brighter the LEDs need to be

CRGB leds[NUM_LEDS]; // Array of all LEDs



void setup() {

    // Safety delay
    delay(2000);

    // FastLED initial setup
    FastLED.addLeds<LED_TYPE,DATA_PIN,COLOR_ORDER>(leds, NUM_LEDS).setCorrection(TypicalLEDStrip);
    FastLED.setMaxPowerInVoltsAndMilliamps(5, 8000);
    FastLED.setBrightness(BRIGHTNESS);

    // Serial communication intialization
    Serial.begin(115200);

    // Pin setup
    pinMode(LED_BUILTIN, OUTPUT);
    pinMode(DATA_PIN, OUTPUT);

    // Making sure all LEDs are stopped
    TurnOffLeds();
}

void loop() {

    // Waiting for user input
    if(Serial.available() > 0){
        input = Serial.read();
    }

    // If 's' is recieved, begin default visualization
    if(input == 's'){
        DefaultVisualization();
    }

    // If 'a' is recieved, begin alternate visualization
    else if(input == 'a'){
        AlternateVisualization();
    }

    // If 'e' is recieved, turn off all LEDs and end visualization
    else if(input == 'e'){
        TurnOffLeds();
    }

    // Set shift speed to [l]ow
    else if (input == 'l'){
        shiftAmount = 1;
    }

    // Set shift speed to [m]edium
    else if (input == 'm'){
        shiftAmount = 2;
    }

    // Set shift speed to [f]ast
    else if (input == 'f'){
        shiftAmount = 3;
    }

    // Set shift speed to [v]ery fast
    else if (input == 'v'){
        shiftAmount = 4;
    }

    // Change the color shift speed
    else if (input - '0' >= 1 && input - '0' <= 9){
        colorShiftSpeed = input - '0';
        colorShiftDelay = colorShiftMaxDelay / colorShiftSpeed;
    }
}



void ReadInput(){ // Reads the user input from the PC application

    // Setup the default value
    input = '0';

    // Wait for serial input
    if(Serial.available() > 0){

        // Write to the input variable
        input = Serial.read();
    }

    // Turn off all LEDs if the [e]nd command or invalid input is recieved
    if(input == 'e' || (input - '0' > 9 || input - '0' < 0)){
        TurnOffLeds();
    }

    // Change the color shift delay if a number is recieved
    else if (input - '0' >= 1 && input - '0' <= 9) {
        colorShiftSpeed = input - '0';
        colorShiftDelay = (colorShiftMaxDelay / colorShiftSpeed);
    }
}


void DefaultVisualization(){ // Launches the default visualization routine

    // Turn on the status LED
    digitalWrite(LED_BUILTIN, HIGH);

    do{
        // Read the user input
        ReadInput();

        // Convert the user input to an integer
        beatStrength = input - '0';

        // Shift all LEDs and light up the recieved amount
        ShiftLeds(beatStrength);

        // Shift the colors
        ShiftColors();

    }while(input != 'e'); // Stop the visualization when [e]nd flag is recieved
}

void ShiftLeds(int ledsToLight){ // Shifts LEDs down the strip (Used for default visualization)

    // If input is recieved (= a beat), light up that many LEDs
    if(ledsToLight > 0){
        for(int i = 0; i < ledsToLight; i++){
            leds[i] = colors;
        }
    }

    // If no input is recieved, turn off the first [shiftAmount] LEDs
    else{
        for(int i = 0; i < shiftAmount; i++){
            leds[i] = CRGB(0, 0, 0);
        }
    }

    // Shift every LED [shiftAmount] spaces down the strip
    for(int i = NUM_LEDS-1; i >= shiftAmount; i--){
        leds[i] = leds[i-shiftAmount];
    }

    // Update the strip
    FastLED.show();
    FastLED.delay(1000/FRAMES_PER_SECOND);
}


void AlternateVisualization(){ // Launches the alternate visualization routine

    // Turn on the status LED
    digitalWrite(LED_BUILTIN, HIGH);

    do{
        // Read the user input
        ReadInput();

        // Turn the brightness down every iteration until it is at the default level
        if (brightnessFactor > 0){
            brightnessFactor -= 1;
        }

        // Convert the user input to an integer
        int beatStrength = input - '0';

        // Check if the brightness can be turned up
        if(brightnessFactor + (beatStrength*3) + BRIGHTNESS <= MAX_BRIGHTNESS){

            // Increase the brightness factor
            brightnessFactor += (beatStrength*3);
        }

        // Set the new brightness
        FastLED.setBrightness(BRIGHTNESS + brightnessFactor);

        // Light up all leds with the new brightness
        LightAllLeds();

        // Shift the colors
        ShiftColors();

    }while(input != 'e');
}

void LightAllLeds(){ // Lights up every LED on the strip (Used for alternate visualization)

    // Setup every LED with the current color
    for(int i = 0; i < NUM_LEDS; i++){
        leds[i] = colors;
    }

    // Update the strip
    FastLED.show();
    FastLED.delay(1000/FRAMES_PER_SECOND);
}


void ShiftColors(){ // Shifts the colors of the strip after a set amount of time

    // Only update the colors if the given time has passed
    if(millis() - lastChange > colorShiftDelay){

        if(colorCounter < 255){ // To pink
            colors.red++;
        }

        else if (colorCounter >= 255 && colorCounter < 510){ // To white
            colors.green++;
        }

        else if (colorCounter >= 510 && colorCounter < 765){ // To yellow
            colors.blue--;
        }

        else if (colorCounter >= 765 && colorCounter < 1020){  // To cyan
            colors.blue++;
            colors.red--;
        }

        else if (colorCounter >= 1020 && colorCounter < 1275){  // Back to blue
            colors.green--;
        }

        else {
            colorCounter = 0;
            colorCounter--;
        }

        colorCounter++;
        lastChange = millis();
    }
}

void TurnOffLeds(){ // Turns off every LED on the strip

    // Turn off the status LED
    digitalWrite(LED_BUILTIN, LOW);

    // Turn off every LED
    for(int i = 0; i < NUM_LEDS ; i++) {
        leds[i] = CRGB(0,0,0);
    }

    // Update the strip
    FastLED.show();
}
