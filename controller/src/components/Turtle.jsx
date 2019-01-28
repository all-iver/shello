import React, { Component } from "react";

import turtleTeal from "../../assets/images/bodies/turtle_body_teal.png";
import turtleBlack from "../../assets/images/bodies/turtle_body_black.png";
import turtleBlue from "../../assets/images/bodies/turtle_body_blue.png";
import turtleBronze from "../../assets/images/bodies/turtle_body_bronze.png";
import turtleGold from "../../assets/images/bodies/turtle_body_gold.png";
import turtleGray from "../../assets/images/bodies/turtle_body_gray.png";
import turtleGreen from "../../assets/images/bodies/turtle_body_green.png";
import turtlePink from "../../assets/images/bodies/turtle_body_pink.png";
import turtlePurple from "../../assets/images/bodies/turtle_body_purple.png";
import turtleRed from "../../assets/images/bodies/turtle_body_red.png";

import turtleLegsLeft from "../../assets/images/Turtle_legs_left_1.png";
import turtleLegsRight from "../../assets/images/Turtle_legs_right_1.png";
import swipeArrows from "../../assets/images/swipe_arrows.png";

import bow from "../../assets/images/turtle_winner_bow.png";

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
    const { swipeArrowsFlipped } = this.state;
    const { showSwipeArrows, body, number, hidden, showBow } = this.props;

    const style = hidden ? { display: "none" } : {};
    const swipeArrowStyle = swipeArrowsFlipped ? { display: "none" } : {};
    const showBowStyle = !showBow ? { display: "none" } : {};

    return (
      <div className={`turtle ${hidden ? "" : "slideInUp"}`} style={style}>
        <img src={turtles[body]} className="body" />
        <img src={bow} style={showBowStyle} className="bow" />
        <div className="numberContainer">
          <p className="number">{number}</p>
        </div>
        <img src={turtleLegsLeft} className="left-legs" />
        <img src={turtleLegsRight} className="right-legs" />
        {showSwipeArrows ? (
          <img
            src={swipeArrows}
            style={swipeArrowStyle}
            className={"swipe-arrows"}
          />
        ) : (
          ""
        )}
      </div>
    );
  }
}

export default Turtle;
