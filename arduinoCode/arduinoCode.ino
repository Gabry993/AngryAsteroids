#include <TimerOne.h>
#include <hapkit3.h>

#define BAUD_RATE 2000000 //921600 // The maximum supported by Windows side

#define RECVBUFSIZE 512

static FILE uartout = {0};

static int uart_putchar (char c, FILE *stream)
{
    Serial.write(c);
    return 0;
}

const hapkit_effect_t potential_well[] = {
  {
    .position = -0.04,
    .width = 0.003,
    .k_spring = 700.0,
    .k_dumper = 5.7,
  },
  {
    .position = -0.02,
    .width = 0.003,
    .k_spring = 700.0,
    .k_dumper = 5.7,
  },
//  {
//    .position = 0.0,
//    .width = 0.010,
//    .k_spring = 1500.0,
//    .k_dumper = 5.7,
//  },
  {
    .position = 0.02,
    .width = 0.003,
    .k_spring = 700.0,
    .k_dumper = 5.7,
  },
  {
    .position = 0.04,
    .width = 0.003,
    .k_spring = 700.0,
    .k_dumper = 5.7,
  },
};

struct Packet
{
  float force;
  float position;
  float velocity;
  float acceleration;
  bool  button;
  uint8_t state;
};

char recvBuf[RECVBUFSIZE] = { '\0' };
int  recvBytes = 0;
bool string_complete = false;
TimerOne timer_tck;
TimerOne send_timer;
Hapkit* hapkit = NULL;
Packet *packet = new Packet();
uint8_t state = 0;

void serialEvent() {
 while (Serial.available() > 0) {
   char chr = Serial.read();
   if (chr == '\n') {
     recvBuf[recvBytes++] = '\0';
     string_complete = true;
     continue;
   }
   recvBuf[recvBytes++] = chr;
 }
}

void snd() {
  Serial.print("Prm ");
  Serial.print(packet->position, 5);
  Serial.print(" ");
  Serial.print(packet->velocity, 5);
  Serial.print(" ");
  Serial.print(packet->acceleration, 5);
  Serial.print(" ");
  Serial.print(!packet->button);
  Serial.print(" ");
  Serial.print(packet->state);
  Serial.print("\n");
}

void snd_ack() {
  Serial.print("Ok");

  Serial.print("\n");
}

void parseMsg() {
//Serial.print(recvBuf);
  //Serial.print(recvBytes);
  recvBytes = 0;

state = (recvBuf[0]) - '0';
  if (recvBuf[0] == 'P' && recvBuf[1] == '1')
  {

    char *header = strtok(recvBuf, " ");
    char *param1 = strtok(NULL, " ");

    state = atoi(param1);


    //snd_ack();

    Serial.print("Ok");
    Serial.print(state);
    Serial.print("\n");

    string_complete = false;
  }
  else
  {
    //Serial.println("Unknown message");
  }
    //Serial.print("Ok\n");
}

int counter = 0;

uint8_t buttonPin = 13;
uint8_t button = 0;
uint8_t lastButton = 0;

uint8_t mode = 0;

float force = 0.0;

void hapticLoop()
{
  hapkit->getSensor()->readSensor();

  if (hapkit->isCalibrated())
  {
    hapkit->update();

//    force = hapkit->calcEffectsForce();

    // Handle buttons events
    button = digitalRead(buttonPin);
    if (button - lastButton > 0) // rising edge
    {
      mode++;
    }
    lastButton = button;

    packet->position = hapkit->getPosition();
    packet->velocity = hapkit->getVelocity();
    packet->acceleration = hapkit->getAcceleration();
    packet->button = button;
    packet->state = state;

    if ((counter++) % 30 == 0)
    {
      snd(); // Send data 10 times slower than the hapticLoop()
      if (string_complete) {
        parseMsg();
      }
    }
  }
}

void setup()
{
  Serial.begin(BAUD_RATE);

  fdev_setup_stream (&uartout, uart_putchar, NULL, _FDEV_SETUP_WRITE);
  stdout = &uartout;

  // Set up button pin
  pinMode(buttonPin, OUTPUT);
  digitalWrite(buttonPin, LOW);
  pinMode(buttonPin, INPUT_PULLUP);

  hapkit = new Hapkit(HAPKIT_YELLOW, 1, A2);

  hapkit->setUpdateRate(1000.0); // 1kHz
  timer_tck.initialize(1000000 / hapkit->getUpdateRate());
  timer_tck.attachInterrupt(hapticLoop);

  hapkit->calibrate();

//  mode = 0;
}

float explosion_pos = NAN;

void loop()
{
  if (state != 3)
  {
    explosion_pos = NAN;
  }
  switch(state)
  {
    case 0:
      hapkit->setForce(-hapkit->getAcceleration() * 0.500 - hapkit->getVelocity() * 0.05); // 500g - Slingshot angle
      break;
    case 1:
      hapkit->setForce(-(hapkit->getPosition() - 0.045) * 50.0 - hapkit->getVelocity() * 2.5); // Slingshot band
      break;
    case 2:
      hapkit->setForce(-hapkit->getAcceleration() * 1.500 - hapkit->getVelocity() * 0.5); // 1500g - Rocket dynamics (angle)
      break;
    case 3:
      /*if (isnan(explosion_pos))
      {
        explosion_pos = hapkit->getPosition();
      }
      else
      {
        float freq = 150.0; // 150Hz-200Hz — typical vibration frequency of a cellphone
        float mass = 1.500; // 1.5kg — rocket mass
        float k_spring = mass * pow(2.0 * M_PI * freq, 2); // k = m * w ^2 = m * (2 * pi * f)^2
        float dx = hapkit->getPosition() - explosion_pos;
        float gap_size = 0.002; // 2 mm
        if (fabs(dx) <  gap_size / 2.0) {
          hapkit->setForce(-dx * k_spring - hapkit->getAcceleration() * mass);
        }
      }*/
       hapkit->setForce(-5000.0);
    case 5:
      hapkit->setForce(force); // Slingshot band with holds
      break;
    case 6:
      hapkit->setForce(0.0); // Free space
      break;

  }
}
