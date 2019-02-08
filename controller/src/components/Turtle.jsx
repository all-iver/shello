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
          // <img
          //   src={swipeArrows}
          //   style={swipeArrowStyle}
          //   className={"swipe-arrows"}
          // />
          <div>
            <div className="dot-container swipe-arrows left">
              <span className="dot" />
            </div>
            <div className="dot-container swipe-arrows right">
              <span className="dot" />
            </div>
          </div>
        ) : (
          ""
        )}
      </div>
    );
  }
}

export default Turtle;
