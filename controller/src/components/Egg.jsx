import React, { Component } from "react";

import egg1 from "../../assets/images/egg_1.png";
import egg2 from "../../assets/images/egg_2.png";
import egg3 from "../../assets/images/egg_3.png";
import egg4 from "../../assets/images/egg_4.png";
import eggShatter from "../../assets/images/egg_shatter.png";
import shadow from "../../assets/images/egg_shadow.png";

import tap1sfx from "../../assets/audio/egg/EggTap1.mp3";
import tap2sfx from "../../assets/audio/egg/EggTap2.mp3";
import tap3sfx from "../../assets/audio/egg/EggTap3.mp3";
import tap4sfx from "../../assets/audio/egg/EggTap4.mp3";
import tap5sfx from "../../assets/audio/egg/EggTap5.mp3";
import tap6sfx from "../../assets/audio/egg/EggTap6.mp3";
import tap7sfx from "../../assets/audio/egg/EggTap7.mp3";
import tap8sfx from "../../assets/audio/egg/EggTap8.mp3";
import tap9sfx from "../../assets/audio/egg/EggTap9.mp3";
import eggShatterSfx from "../../assets/audio/egg/EggCrack1.mp3";

function pickRandomlyFromArray(array) {
  return array[Math.floor(Math.random() * array.length)];
}

class Egg extends Component {
  constructor(props) {
    super(props);

    this.state = {
      taps: 0,
      cracks: 0,
      wobbling: false
    };

    this.audio = {
      tap0: new Audio(tap1sfx),
      tap1: new Audio(tap2sfx),
      tap2: new Audio(tap3sfx),
      tap3: new Audio(tap4sfx),
      tap4: new Audio(tap5sfx),
      tap5: new Audio(tap6sfx),
      tap6: new Audio(tap7sfx),
      tap7: new Audio(tap8sfx),
      tap8: new Audio(eggShatterSfx)
    };
  }
  render() {
    const { ready, crackable = true } = this.props;
    const { taps, wobbling, cracks } = this.state;

    let frame;

    switch (true) {
      case Boolean(taps >= 3): // needs to be +1 of the onClick taps because its triggered by them
        frame = egg4;
        break;

      case Boolean(taps >= 2):
        frame = egg3;
        break;

      case Boolean(taps >= 1):
        frame = egg2;
        break;

      default:
      case Boolean(taps === 0):
        frame = egg1;
        break;
    }

    const shattering = taps >= 3;

    if (shattering) {
      setTimeout(() => ready(), 1000);
    }

    return (
      <div className="egg">
        <img
          src={eggShatter}
          className={`eggShatter${shattering ? " shattering slideOutUp" : ""}`}
        />
        <img
          src={frame}
          onClick={() => {
            this.audio[`tap${taps}`]
              ? this.audio[`tap${taps}`].play()
              : this.audio.tap0.play();

            this.setState(prevState => ({
              wobbling: true,
              taps: crackable ? prevState.taps + 1 : prevState.taps
            }));
          }}
          className={
            wobbling
              ? pickRandomlyFromArray([
                  "eggSprite wobble-short-left",
                  "eggSprite wobble-short-right"
                ])
              : "eggSprite"
          }
          onAnimationEnd={() => {
            this.setState({
              wobbling: false,
              cracks: cracks + 1
            });
          }}
        />
        <img src={shadow} className="shadow" />
      </div>
    );
  }
}

export default Egg;
