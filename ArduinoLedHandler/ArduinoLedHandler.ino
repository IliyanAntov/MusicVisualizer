
void setup() {
  Serial.begin(38400);
  pinMode(LED_BUILTIN, OUTPUT);
}

void loop() {
  String str;
  if(Serial.available() > 0){
    str = Serial.readString();
  }
  if(str == "b"){
    digitalWrite(LED_BUILTIN, HIGH);
    delay(1000);
    digitalWrite(LED_BUILTIN, LOW);
  }
}
