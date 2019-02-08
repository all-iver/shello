import React, { Component } from "react";
import Swipe from "react-easy-swipe";
import AirConsole from "air-console";

const distance = (a, b) =>
  Math.sqrt(Math.pow(a.x - b.x, 2) + Math.pow(a.y - b.y, 2));

class Flipper extends Component {
  constructor(props) {
    super(props);

    this.state = {
      start: undefined
    };
  }

  render() {
    const { airconsole, side, sendMessage } = this.props;

    return (
      <Swipe
        onSwipeStart={evt => {
          this.setState({
            start: {
              x: evt.changedTouches[0].pageX,
              y: evt.changedTouches[0].pageY
            }
          });
        }}
        // onSwipeMove={console.log}
        onSwipeEnd={evt => {
          evt.preventDefault();

          console.log(
            distance(this.state.start, {
              x: evt.changedTouches[0].pageX,
              y: evt.changedTouches[0].pageY
            })
          );

          // airconsole.message(AirConsole.SCREEN, {
          //   action: `swipe${side}End`
          // });

          sendMessage({
            action: `swipe${side}End`,
            distance: distance(this.state.start, {
              x: evt.changedTouches[0].pageX,
              y: evt.changedTouches[0].pageY
            })
          });
        }}
      >
        <div className="flipper" />
      </Swipe>
    );
  }
}

export default Flipper;
