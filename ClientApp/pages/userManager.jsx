import React, { useEffect, useState, useContext } from 'react';
import styles from '../styles/UserManager.module.css'
import Link from 'next/link'
import UserCard from '../components/UserCard';
import UserFilter from '../components/UserFilter';
import ErrorText from '../components/ErrorText';
import { LoggedContext } from '../context/loggedContext';
import StyledInput from '../components/StyledInput';
import { faSearch } from '@fortawesome/free-solid-svg-icons'

const UserManager = (props) => {
	useEffect(() => props.addBackground(`
    linear-gradient(135deg, rgb(163, 255, 220) 0%, rgba(29, 185, 181, 1) 41%, rgb(219, 129, 215) 100%)
	`), []);

	const logged = useContext(LoggedContext);
	const [filteredUsersIndex, setFilteredUsersIndex] = useState([]);
	const [users, setUsers] = useState([]);
	const [error, setError] = useState([]);
  const [searchQuery, setSearchQuery] = useState("");

	useEffect(async () => {
		if (logged.info.isAdmin) {
			const res = await fetch('api/Users/GetUsers');

			switch (res.status) {
				case 200: {
					setUsers(await res.json());
					setError(``);
					break;
				}
				case 401: {
					setError(`You're Unauthorized to view this page`);
					setUsers([]);
					break;
				}
				default: {
					setError(`${res.status} error`);
					setUsers([]);
				}
			}
		} else {
			setError(`You're Unauthorized to view this page`);
			setUsers([]);
		}
	}, [logged]);

	useEffect(() => {
		setFilteredUsersIndex(users.map((_, i) => i).filter(userIndex => {
			const user = users[userIndex];
			return Object.keys(user).some((key) => {
				return user[key].toString().toLowerCase().includes(searchQuery.toLowerCase());
			});
		}));
	}, [users, searchQuery])

	return (
		<div className={styles.container}>
			<ErrorText>
				{error}
			</ErrorText>
			<div className={styles.viewContainer}>
				<div className={styles.searchContainer}>
					<StyledInput visible={true} type={"text"}
						icon={faSearch} state={[searchQuery, setSearchQuery]} />
				</div>
				<div className={styles.cardsContainer}>
					{
						users.map((user, i) => {
							return <UserCard canDelete={user.Username !== logged.info.username} user={user} key={i} visible={filteredUsersIndex.includes(i)} />;
						})
					}
				</div>
			</div>
		</div>
	)
}

export default UserManager;