import React, { useEffect } from 'react';
import styles from '../styles/Home.module.css'
import Link from 'next/link'
import Image from 'next/image'

const Home = (props) => {

	const mainButtons = [
		{ text: "Learn to Punch", important: true, link: 'learn' },
		{ text: "News", important: false, link: 'news' },
	]

	useEffect(() => props.addBackground(`
	linear-gradient(135deg, rgb(163, 185, 255) 0%, rgba(29, 185, 181, 1) 41%, rgb(49, 147, 142) 100%)
	`), [])

	return (
		<div>
			<div className={styles.topSection}>
				{/* <div className={styles.title}>{"BOXING"}</div> */}
				<div className={styles.imgTitleContainer}>
					<Image src={"/Boxing.png"} width={601} height={211} />
				</div>
				<TopButtons buttons={mainButtons} />
			</div>
		</div>
	)
}

const TopButtons = (props) => {
	return (
		<div className={styles.topButtonsContainer}>
			{
				props.buttons.map((button, i) => {
					return (
						<div key={i}>
							<Link href={button.link}>
								<button className={button.important ? styles.important : ""}>
									{button.text}
								</button>
							</Link>
						</div>
					)
				})
			}
		</div>
	)
}
export default Home;