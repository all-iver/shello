import React, { Component } from "react";
import Swipe from "react-easy-swipe";
import AirConsole from "air-console";

class Flipper extends Component {
  render() {
    const { airconsole, side } = this.props;

    return (
      <Swipe
        onSwipeStart={console.log}
        onSwipeMove={console.log}
        onSwipeEnd={() => {
          airconsole.message(AirConsole.SCREEN, {
            action: `swipe${side}End`
          });
        }}
      >
        <div className="flipper" />
      </Swipe>
    );
  }
}

export default Flipper;
