import React, { Component } from "react";

import turtleBody from "../../assets/images/turtle_body_teal.png";
import turtleLegsLeft from "../../assets/images/Turtle_legs_left 1.png";
import turtleLegsRight from "../../assets/images/Turtle_legs_right 1.png";
import swipeArrows from "../../assets/images/swipe arrows.png";

class Turtle extends Component {
  constructor(props) {
    super(props);

    this.state = {
      swipeArrowsFlipped: false
    };

    setInterval(
      () =>
        this.setState({
          swipeArrowsFlipped: !this.state.swipeArrowsFlipped
        }),
      1000
    );
  }
  render() {
    const { showSwipeArrows } = this.props;
    return (
      <div className="turtle">
        <img src={turtleBody} className="body" />
        <img src={turtleLegsLeft} className="left-legs" />
        <img src={turtleLegsRight} className="right-legs" />
        {showSwipeArrows ? (
          <img
            src={swipeArrows}
            className={`swipe-arrows ${
              this.state.swipeArrowsFlipped ? "flip" : ""
            }`}
          />
        ) : (
          ""
        )}
      </div>
    );
  }
}

export default Turtle;
