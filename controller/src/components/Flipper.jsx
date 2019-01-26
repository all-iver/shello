import React, { Component } from "react";
import Swipe from "react-easy-swipe";
import AirConsole from "air-console";

class Flipper extends Component {
  render() {
    const { airconsole } = this.props;

    return (
      <Swipe
        onSwipeStart={console.log}
        onSwipeMove={console.log}
        onSwipeEnd={() => {
          airconsole.message(AirConsole.SCREEN, {
            action: "swipeEnd"
          });
        }}
      >
        <div className="flipper" />
      </Swipe>
    );
  }
}

export default Flipper;
