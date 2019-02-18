import React, { Component } from "react";

import turtleTeal from "../../assets/images/bodies/turtle_teal_1.png";
import turtleBlack from "../../assets/images/bodies/turtle_black_1.png";
import turtleBlue from "../../assets/images/bodies/turtle_blue_1.png";
import turtleBronze from "../../assets/images/bodies/turtle_bronze_1.png";
import turtleGold from "../../assets/images/bodies/turtle_gold_1.png";
import turtleGray from "../../assets/images/bodies/turtle_gray_1.png";
import turtleGreen from "../../assets/images/bodies/turtle_green_1.png";
import turtlePink from "../../assets/images/bodies/turtle_pink_1.png";
import turtlePurple from "../../assets/images/bodies/turtle_purple_1.png";
import turtleRed from "../../assets/images/bodies/turtle_red_1.png";

import turtleLegsLeft from "../../assets/images/turtle_legs_left_2.png";
import turtleLegsRight from "../../assets/images/turtle_legs_right_2.png";
import turtleLegsLeftClosed from "../../assets/images/turtle_legs_left_1.png";
import turtleLegsRightClosed from "../../assets/images/turtle_legs_right_1.png";

import bow from "../../assets/images/turtle_winner_bow.png";
import circle from "../../assets/images/circle.png";

const turtles = {
  teal: turtleTeal,
  black: turtleBlack,
  blue: turtleBlue,
  bronze: turtleBronze,
  gold: turtleGold,
  gray: turtleGray,
  green: turtleGreen,
  pink: turtlePink,
  purple: turtlePurple,
  red: turtleRed
};

const Dot = ({ pulse, side }) => (
  <div
    className={`dot-container swipe-arrows ${
      side === "left" ? "left" : "right"
    }`}
  >
    <img src={circle} className={`${pulse ? "pulse" : "hidden"}`} />
    <p className={`${pulse ? "scaleUp" : ""}`}>Tap!</p>
  </div>
);

function getSwipeImage({ showSwipeArrows, swipe, side }) {
  // This means the turtle cannot move right now, so don't show swipes
  if (!showSwipeArrows) {
    switch (side) {
      case "left":
        return turtleLegsLeft;
      case "right":
        return turtleLegsRight;
    }
  }

  switch (swipe) {
    case "swipeLeftEnd":
      switch (side) {
        case "left":
          return turtleLegsLeftClosed;
        case "right":
          return turtleLegsRight;
      }
    case "swipeRightEnd":
      switch (side) {
        case "left":
          return turtleLegsLeft;
        case "right":
          return turtleLegsRightClosed;
      }
    case "noSwipe":
      switch (side) {
        case "left":
          return turtleLegsLeft;
        case "right":
          return turtleLegsRight;
      }
  }
}

class Turtle extends Component {
  constructor(props) {
    super(props);

    this.state = {
      leftSide: false
    };

    setInterval(
      () =>
        this.setState({
          leftSide: !this.state.leftSide
        }),
      750
    );
  }

  render() {
    const { swipeArrowsFlipped, leftSide } = this.state;
    const {
      showSwipeArrows,
      body,
      number,
      hidden,
      showBow,
      swipe
    } = this.props;

    const style = hidden ? { display: "none" } : {};
    const swipeArrowStyle = swipeArrowsFlipped ? { display: "none" } : {};
    const showBowStyle = !showBow ? { display: "none" } : {};

    if (swipe) {
      // TODO: change in sync to this
    }

    return (
      <div className={`turtle ${hidden ? "" : "slideInUp"}`} style={style}>
        <img src={turtles[body]} className="body" />
        <img src={bow} style={showBowStyle} className="bow" />
        <div className="numberContainer">
          <p className="number">{number}</p>
        </div>
        <img
          src={getSwipeImage({ showSwipeArrows, swipe, side: "left" })}
          className="left-legs"
        />
        <img
          src={getSwipeImage({ showSwipeArrows, swipe, side: "right" })}
          className="right-legs"
        />
        {showSwipeArrows ? (
          // <img
          //   src={swipeArrows}
          //   style={swipeArrowStyle}
          //   className={"swipe-arrows"}
          // />
          <div>
            <Dot side="left" pulse={leftSide} />
            <Dot side="right" pulse={!leftSide} />
          </div>
        ) : (
          ""
        )}
      </div>
    );
  }
}

export default Turtle;
