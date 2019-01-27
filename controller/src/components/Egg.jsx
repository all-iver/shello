import React, { Component } from "react";

import egg1 from "../../assets/images/egg_1.png";
import egg2 from "../../assets/images/egg_2.png";
import egg3 from "../../assets/images/egg_3.png";
import egg4 from "../../assets/images/egg_4.png";
import eggShatter from "../../assets/images/egg_shatter.png";
import shadow from "../../assets/images/egg_shadow.png";
import { read } from "fs";

function pickRandomlyFromArray(array) {
  return array[Math.floor(Math.random() * array.length)];
}

class Egg extends Component {
  constructor(props) {
    super(props);

    this.state = {
      taps: 0,
      cracks: 0,
      wobbling: false,
      shattering: false
    };
  }
  render() {
    const { ready } = this.props;
    const { taps, wobbling, cracks, shattering } = this.state;

    let frame;

    switch (true) {
      case Boolean(taps >= 9): // needs to be +1 of the onClick taps because its triggered by them
        frame = egg4;
        break;

      case Boolean(taps >= 5):
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

    return (
      <div className="egg">
        <img
          src={eggShatter}
          className={`eggShatter${shattering ? " shattering slideOutUp" : ""}`}
        />
        <img
          src={frame}
          onClick={() => {
            if (taps >= 8) {
              this.setState({
                shattering: true
              });

              setTimeout(() => ready(), 1000);
              // ready();
            }

            this.setState(prevState => ({
              wobbling: true,
              taps: prevState.taps + 1
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
