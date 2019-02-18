import React, { Component } from "react";
import AirConsole from "air-console";
import Turtle from "../components/Turtle";

/**
 * @author Salman A - https://stackoverflow.com/users/87015/salman-a
 * https://stackoverflow.com/questions/13627308/add-st-nd-rd-and-th-ordinal-suffix-to-a-number
 * @param {number} number
 */
function getOrdinalSuffix(number) {
  const onesRemainder = number % 10;
  const tensRemainder = number % 100;

  if (onesRemainder === 1 && tensRemainder !== 11) {
    return "st";
  }

  if (onesRemainder === 2 && tensRemainder !== 12) {
    return "nd";
  }

  if (onesRemainder === 3 && tensRemainder !== 13) {
    return "rd";
  }

  return "th";
}

class VictoryView extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    const { time, place, body, number, showBow } = this.props;

    return (
      <div id="victoryView">
        <Turtle
          showSwipeArrows={false}
          body={body}
          number={number}
          showBow={showBow}
        />
        <div className="victoryViewTextContainer">
          <div className="placingContainer">
            <p>{time ? `${place}${getOrdinalSuffix(place)}` : ""}</p>
            <p>{place ? `${time.toFixed(2)}s` : ""}</p>
          </div>
          <p>Waiting for the next race to start...</p>
        </div>
      </div>
    );
  }
}

export default VictoryView;
