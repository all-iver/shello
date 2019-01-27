import React, { Component } from "react";

import egg from "../../assets/images/egg_1.png";
import shadow from "../../assets/images/egg_shadow.png";

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
  }
  render() {
    const { ready } = this.props;
    const { taps, wobbling, cracks } = this.state;

    return (
      <div className="egg">
        <img
          src={egg}
          onClick={() => {
            if (taps >= 8) {
              ready();
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
